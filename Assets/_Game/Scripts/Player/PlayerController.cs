using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.SceneManagement;
public interface IPlayerController
{
    public sbyte JumpSide { get; }
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

    [Header("Jump Settings")]
    public float JumpForceUp = 15f;
    public float StartJumpForceUp = 7f;
    public float JumpForceSide = 3f;
    public float JumpTime = 0.5f;
    public float CoolDownTimeBetweenJumps = 0.05f;
    public float GravityForceWhileFalling = 3f;
    public float SpeedOnFloor = 4f;

    public event Action OnJump;
    public event Action<sbyte> OnDoubleJump;
    public event Action OnWallTouched;
    public event Action OnFloorTouched;
    public event Action OnFloorLeft;

    private sbyte jumpSide = 1;
    private bool isDead;
    [SerializeField] private bool isOnWall;
    private bool isOnFloor;
    private bool canJump;
    private bool canDoubleJump;
    private float jumpTimeCounter;
    private BaseWall currentWall;
    private Rigidbody2D rb;

    private IPlayerInput Input;
    public sbyte JumpSide => jumpSide;

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
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

    #region Methods
    private void ResetJump()
    {
        canJump = true;
        canDoubleJump = false;
    }
    public void UpdateJumpSide()
    {
        jumpSide = (sbyte)(rb.linearVelocity.x > 0 ? 1 : -1);
    }
    private void FlipScale(sbyte side)
    {
        transform.localScale = new Vector3(side, transform.localScale.y, transform.localScale.z);
    }
    public void Jump()
    {
        canJump = false;
        canDoubleJump = true;

        OnJump?.Invoke();
        ApplyJumpForce(jumpSide);

        if (currentWall != null && currentWall.TypeOfWall == WallType.Moving)
        {
            currentWall.Left(this);
            rb.gravityScale = 2f;
            isOnWall = false;
        }
    }
    private void DoubleJump()
    {
        canDoubleJump = false;
        UpdateJumpSide();
        sbyte doubleSide = (sbyte)-jumpSide;
        jumpSide = doubleSide;

        OnDoubleJump?.Invoke(jumpSide);
        ApplyJumpForce(jumpSide);
        FlipScale(jumpSide);

    }
    private void ApplyJumpForce(sbyte side)
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 2f;

        rb.AddForce(new Vector2(JumpForceSide * side, StartJumpForceUp), ForceMode2D.Impulse);
        jumpTimeCounter = 0f;

    }
    public void VerticalJump()
    {
        if (isOnWall) return;
        jumpTimeCounter += Time.deltaTime;
        if (jumpTimeCounter < JumpTime)
            rb.AddForce(Vector2.up * JumpForceUp * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void JumpFromBounce()
    {
        UpdateJumpSide();
        FlipScale(jumpSide);
        Jump();
    }
    #endregion

    #region Event Handle
    public void HandleTouchBegan()
    {
        //Debug.Log($"[Tap] canJump={canJump}, canDoubleJump={canDoubleJump}, t={Time.realtimeSinceStartup:F3}");
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
    private void HandleDeath()
    {
        isDead = true;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }
    public void HandleRespawn()
    {
        Initialize();
        if (rb.simulated == false)
        {
            rb.simulated = true;
        }
    }
    #endregion


    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
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
            rb.position +=
                Vector2.down * currentWall.SpeedOfPlayerFriction * Time.deltaTime;

        if (rb.linearVelocity.y < -0.2f && !isOnWall)
            rb.gravityScale = GravityForceWhileFalling;

    }
    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Wall") && !isOnWall)
        {
            Debug.Log(collision.gameObject.tag);
            if (collision.contacts[0].normal.y < 0)
            {
                jumpTimeCounter = JumpTime;

                rb.linearVelocity = Vector2.zero;
                //  UpdateJumpSide();
                rb.AddForce(new Vector2(JumpForceSide * jumpSide, 0), ForceMode2D.Impulse);
                return;
            }
            //  if (collision.contacts[0].normal.y < 0)
            //  {
            //      jumpTimeCounter = JumpTime;
            //      return;
            //  }

            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            isOnWall = true;
            isOnFloor = false;

            currentWall = collision.gameObject.GetComponent<BaseWall>();
            float xDiff = transform.position.x - collision.transform.position.x;
            jumpSide = (sbyte)(xDiff > 0 ? 1 : -1);

            ResetJump();
            currentWall.Touched(this);
            OnWallTouched?.Invoke();
            if (collision.contacts[0].normal.y > 0 && currentWall.FixIfOnTop)
            {
                // UpdateJumpSide();
                if (currentWall.OnlyLeftWall) jumpSide = -1;
                if (currentWall.OnlyRightWall) jumpSide = 1;

                StartCoroutine(BringPlayerOnPlatform(new Vector2(
                    collision.transform.position.x +
                    (jumpSide * Math.Abs(transform.lossyScale.x) * 0.4f),
                    transform.position.y - 0.5f
                )));
                //(-JumpSide * Math.Abs(transform.lossyScale.x) * 0.5f),
            }

            FlipScale(jumpSide);
        }

        if (collision.gameObject.CompareTag("Floor") && !isOnWall)
        {
            Debug.Log(collision.gameObject.tag);
            if (transform.position.y - collision.transform.position.y > 0)
            {
                isOnFloor = true;
                ResetJump();
            }

            OnFloorTouched?.Invoke();
            if (collision.contacts[0].normal.y == 0)
            {
                StartCoroutine(BringPlayerOnPlatform(new Vector2(
                    transform.position.x + 0.3f * jumpSide,
                    collision.transform.position.y + 0.3f
                )));
            }
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Wall") && isOnWall)
        {
            if (currentWall != null && currentWall.TypeOfWall == WallType.Moving) return;
            rb.gravityScale = 2f;
            isOnWall = false;
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

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        if (isOnWall && collision.gameObject.CompareTag("Wall") && currentWall != null && !isOnFloor)
        {
            currentWall.Staying(this);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            isDead = true;
            PlayerManager.Instance.OnDie();
        }
    }

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

}
