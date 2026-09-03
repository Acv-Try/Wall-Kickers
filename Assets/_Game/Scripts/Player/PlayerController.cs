using System;
using System.Collections;
using UnityEngine;

public interface IPlayerController
{
    public event Action OnJump;
    public event Action<sbyte> OnDoubleJump;
    public event Action OnWallTouched;
    public event Action OnFloorTouched;
    public event Action OnFloorLeft;
    public void Initialize();
    public void HandleTouchBegan();
    public void HandleTouchHeld();
}
public class PlayerController : MonoBehaviour, IPlayerController
{
    #region Variables
    [Header("Jump Settings")]
    [SerializeField] private float JumpForceUp = 20f;
    [SerializeField] private float StartJumpForceUp = 7f;
    [SerializeField] private float JumpForceSide = 4f;
    [SerializeField] private float JumpTime = 0.5f;
    [SerializeField] private float CoolDownTimeBetweenJumps = 0.05f;
    [SerializeField] private float GravityForceWhileFalling = 5f;
    [SerializeField] private float SpeedOnFloor = 4.5f;
    [Header("Wall/Floor Priority")]
    [SerializeField] private LayerMask wallFloorMask;
    [SerializeField] private float overlapPadding = 0.08f;
    [Header("Wall Top Landing")]
    [SerializeField] private float wallStickOverlapRatio = 1f / 3f;
    [SerializeField] private float floorEdgeNudgeRatio = 1f / 3f;

    public event Action OnJump;
    public event Action<sbyte> OnDoubleJump;
    public event Action OnWallTouched;
    public event Action OnFloorTouched;
    public event Action OnFloorLeft;

    private sbyte jumpSide = 1;

    private bool isDead;
    private bool isOnWall;
    private bool isOnFloor;
    private bool canJump;
    private bool canDoubleJump;

    private float jumpTimeCounter;

    private BaseWall currentWall;
    private Rigidbody2D rb;

    private const int MaxOverlapResults = 8;
    private BoxCollider2D boxCollider;
    private readonly Collider2D[] overlapBuffer = new Collider2D[MaxOverlapResults];
    private ContactFilter2D contactFilter;

    private IPlayerInput Input;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(wallFloorMask);
        contactFilter.useTriggers = false; //set true only if wall/floor colliders are triggers, not solid colliders
    }

    #region Public Methods
    public void Initialize()
    {

        isDead = false;

        canJump = true;
        canDoubleJump = false;

        isOnWall = true;
        isOnFloor = false;

        jumpSide = 1;
        jumpTimeCounter = 0f;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        FlipScale(jumpSide);
    }
    public void JumpFromBounce()
    {
        ResetJump();
        UpdateJumpSide();
        FlipScale(jumpSide);
        Jump();
    }
    #endregion

    #region Methods
    private void ResetJump()
    {
        canJump = true;
        canDoubleJump = false;
    }
    private void UpdateJumpSide()
    {
        jumpSide = (sbyte)(rb.linearVelocity.x > 0 ? 1 : -1);
    }
    private void FlipScale(sbyte side)
    {
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * side, transform.localScale.y, transform.localScale.z);
    }
    private void Jump()
    {
        canJump = false;
        canDoubleJump = true;

        OnJump?.Invoke();
        ApplyJumpForce(jumpSide);

        if (currentWall != null && currentWall.Type == WallType.Moving)
        {
            currentWall.Left(this);
            rb.gravityScale = 2f;
            isOnWall = false;
        }
    }
    private void DoubleJump()
    {
        canDoubleJump = false;
        FlipScale(jumpSide);
        UpdateJumpSide();
        sbyte doubleSide = (sbyte)-jumpSide;
        jumpSide = doubleSide;

        OnDoubleJump?.Invoke(jumpSide);
        ApplyJumpForce(jumpSide);

    }
    private void ApplyJumpForce(sbyte side)
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 2f;

        rb.AddForce(new Vector2(JumpForceSide * side, StartJumpForceUp), ForceMode2D.Impulse);
        jumpTimeCounter = 0f;

    }
    private void VerticalJump()
    {
        if (isOnWall) return;
        jumpTimeCounter += Time.deltaTime;
        if (jumpTimeCounter < JumpTime)
            rb.AddForce(Vector2.up * JumpForceUp * Time.deltaTime, ForceMode2D.Impulse);
    }

    #endregion

    #region Event Handle
    public void HandleTouchBegan()
    {
        if (canJump)
        {
            Jump();
            return;
        }

        if (canDoubleJump)
        {
            DoubleJump();
        }
    }
    public void HandleTouchHeld()
    {
        VerticalJump();
    }
    private void HandleFloorLanding(Collider2D floorCollider)
    {
        Debug.Log("handle");
        rb.gravityScale = 0f;
        isOnFloor = true;
        isOnWall = false;
        currentWall = null;

        ResetJump();
        OnFloorTouched?.Invoke();

        Debug.Log(boxCollider.bounds.extents.y);
        StartCoroutine(BringPlayerOnPlatform(new Vector2(
            transform.position.x,
            floorCollider.bounds.max.y + boxCollider.bounds.extents.y
        )));
    }
    #endregion

    #region FixedUpdate
    private void FixedUpdate()
    {
        if (isDead) return;
        PlayerManager.Instance.RiseOnPlayerPositionChange(transform.position);
        if (isOnFloor)
        {
            rb.linearVelocity =
                new Vector2(SpeedOnFloor * jumpSide, rb.linearVelocity.y);
        }

        if (isOnWall && currentWall != null)
            rb.position += Vector2.down * currentWall.PlayersFrictionSpeed * Time.deltaTime;

        if (rb.linearVelocity.y < -0.2f && !isOnWall)
            rb.gravityScale = GravityForceWhileFalling;

    }
    #endregion

    #region Player Collision Logic Handler
    private bool TryGetLandingFloor(out Collider2D floorCollider)
    {
        floorCollider = null;
        Vector2 size = boxCollider.size + new Vector2(overlapPadding, overlapPadding);
        Vector2 center = (Vector2)transform.position + boxCollider.offset;

        int count = Physics2D.OverlapBox(center, size, 0f, contactFilter, overlapBuffer);

        for (int i = 0; i < count; i++)
        {
            var col = overlapBuffer[i];
            if (col == null || !col.CompareTag("Floor")) continue;

            Bounds playerBounds = boxCollider.bounds;
            Bounds floorBounds = col.bounds;

            bool horizontalOverlap =
               playerBounds.max.x > floorBounds.min.x &&
            playerBounds.min.x < floorBounds.max.x;

            bool nearFloorTop = playerBounds.min.y >= floorBounds.max.y - overlapPadding;

            if (horizontalOverlap && nearFloorTop)
            {
                Debug.Log(col.gameObject.tag);
                floorCollider = col;
                return true;
            }
        }
        return false;
    }
    private bool IsWallNeedOnTouch(BaseWall wall)
    {
        if (wall.Type == WallType.Bounce) return false;
        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Wall") && !isOnWall)
        {
            float contactNormalY = collision.contacts[0].normal.y;
            if (contactNormalY < 0)
            {
                if (TryGetLandingFloor(out Collider2D landingFloor))
                {
                    HandleFloorLanding(landingFloor);
                }
                return;
            }

            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            isOnWall = true;
            isOnFloor = false;

            currentWall = collision.gameObject.GetComponent<BaseWall>();
            float xDiff = transform.position.x - collision.transform.position.x;
            jumpSide = (sbyte)(xDiff > 0 ? 1 : -1);

            ResetJump();
            currentWall.Touched(this);
            if (IsWallNeedOnTouch(currentWall))
                OnWallTouched?.Invoke();

            if (contactNormalY > 0 && currentWall.FixIfOnTop)
            {
                Bounds wallBounds = collision.collider.bounds;
                float wallCenterX = wallBounds.center.x;
                float wallHalfWidth = wallBounds.extents.x;
                float playerHalfWidth = boxCollider.bounds.extents.x;

                sbyte side = (transform.position.x >= wallCenterX) ? (sbyte)1 : (sbyte)-1;

                if (currentWall.OnlyLeftWall) jumpSide = -1;
                if (currentWall.OnlyRightWall) jumpSide = 1;

                float stickOffset = playerHalfWidth * wallStickOverlapRatio;
                float targetX = wallCenterX + side * (wallHalfWidth - stickOffset);

                StartCoroutine(BringPlayerOnPlatform(new Vector2(
                targetX,
                transform.position.y - 0.5f
                )));
            }
            FlipScale(jumpSide);

        }

        if (collision.gameObject.CompareTag("Floor") && !isOnWall)
        {
            if (transform.position.y - collision.transform.position.y > 0)
            {
                isOnFloor = true;
                ResetJump();
            }
            FlipScale(jumpSide);
            OnFloorTouched?.Invoke();
            if (collision.contacts[0].normal.y == 0)
            {
                Bounds floorBounds = collision.collider.bounds;
                float playerHalfWidth = boxCollider.bounds.extents.x;
                float edgeMargin = playerHalfWidth * floorEdgeNudgeRatio;

                float clampedX = Mathf.Clamp(
                    transform.position.x,
                    floorBounds.min.x + edgeMargin,
                    floorBounds.max.x - edgeMargin
                );

                StartCoroutine(BringPlayerOnPlatform(new Vector2(
                    clampedX,
                    floorBounds.max.y + boxCollider.bounds.extents.y
                )));
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Wall") && isOnWall)
        {
            if (currentWall != null && currentWall.Type == WallType.Moving) return;
            rb.gravityScale = 2f;
            isOnWall = false;
            //Debug.Log("PlayerLeftTheWall");
            currentWall?.Left(this);
            currentWall = null;
        }
        else if (collision.gameObject.CompareTag("Floor") && isOnFloor)
        {
            rb.gravityScale = 2f;
            isOnFloor = false;
            OnFloorLeft?.Invoke();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        if (isOnWall && collision.gameObject.CompareTag("Wall") && currentWall != null && !isOnFloor)
        {
            currentWall.Staying(this);
        }
    }
    #endregion

    #region Death Trigger Handler
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            isDead = true;
            PlayerManager.Instance.OnDie();
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(CoolDownTimeBetweenJumps);
        if (!isOnWall) canDoubleJump = true;
    }
    private IEnumerator BringPlayerOnPlatform(Vector2 targetPosition)
    {
        rb.simulated = false;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10f;
            transform.position = Vector2.Lerp(transform.position, targetPosition, t);
            if (t > 0.8f) rb.simulated = true;
            yield return null;
        }
    }
    #endregion
}
