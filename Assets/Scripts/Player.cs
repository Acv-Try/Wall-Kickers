using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
public class Player : MonoBehaviour
{

    [Header("Jump Settings")]

    [Tooltip("Force that applies every frame while the jump button is held down")]
    public float JumpForceUp = 15f;

    [Tooltip("Starting force applied when the jump button is first pressed")]
    public float StartJumpForceUp = 7;

    [Tooltip("Force applied to the side when the jump button is first pressed")]
    public float JumpForceSide = 3f;

    [Tooltip("Maximum time the jump button can be held")]
    public float JumpTime = 0.5f;

    [Tooltip("Time after a jump before the player can make second jump")]
    public float CoolDownTimeBetweenJumps = 0.2f;

    [Tooltip("Gravity scale applied when the player is falling")]
    public float GravityForceWhileFalling = 3;

    [Tooltip("Speed of the player when on the floor")]
    public float SpeedOnFloor = 5f;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator puffEffect;

    public sbyte jumpSide = 1; // a variable to determine the direction of the jump, 1 for right and -1 for left. It is set when the player collides with a wall.
    [SerializeField]
    private bool CanJump = true;
    [SerializeField]
    private bool OnWall = false;
    [SerializeField]
    private bool OnFloor = false;
    [SerializeField]
    private bool CanDoubleJump = false;
    [SerializeField]
    private sbyte LinearVelocityX;

    [SerializeField] private float stepHeight;
    private float lastCheckpointY;
    private int checkPoint;
    private bool isCheckedPoints;

    float JumpTimeCounter;
    public Rigidbody2D rb;

    public Wall CurrentWall;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastCheckpointY = transform.position.y;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CanJump)
            {
                JumpEffect();
                Jump();
            }

            if (CanDoubleJump)
            {
                playerAnimator.SetBool("isBackFlip", true);
                StartJump((sbyte)-jumpSide);
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                CanDoubleJump = false;
            }
        }

        if (Input.GetMouseButton(0) && !OnWall)
        {
            VerticalJump();
        }

        if (rb.linearVelocity.y < -0.2f && !OnWall) // makes a gravity stronger then player is falling 
        {
            rb.gravityScale = GravityForceWhileFalling;
        }

        if (OnWall) // applies friction to the player when on the wall so the player is slides down from the wall
        {
            rb.position += Vector2.down * CurrentWall.SpeedOfPlayerFriction * Time.deltaTime;
        }
        if (transform.position.y >= lastCheckpointY + stepHeight)
        {
            lastCheckpointY = transform.position.y;

            checkPoint++;
            GridGenerator.Instance.CheckIfPlayerAboveOfMiddleLevel(transform.position, checkPoint);
        }
    }

    void FixedUpdate()
    {
        if (OnFloor)
        {
            rb.linearVelocity = new Vector2(SpeedOnFloor * jumpSide * Time.deltaTime, rb.linearVelocity.y);
        }
    }

    public void Jump()
    {
        playerAnimator.SetBool("isJump", true);
        StartJump(jumpSide);
        CanJump = false;
        if (CurrentWall.TypeOfWall == WallType.Moving)
        {
            CurrentWall.Left(this);
            rb.gravityScale = 2f;
            OnWall = false;
        }
        StartCoroutine(CoolDown());

    }

    void StartJump(sbyte side) // Method to start the jump
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 2f;

        rb.AddForce(new Vector2(JumpForceSide * side, StartJumpForceUp), ForceMode2D.Impulse);
        JumpTimeCounter = 0;

        LinearVelocityX = (sbyte)rb.linearVelocity.x;
    }

    void VerticalJump() // Method for the vertical part of the jump that applies force while the jump button pressed
    {
        JumpTimeCounter += Time.deltaTime;

        if (JumpTimeCounter < JumpTime)
        {
            rb.AddForce(Vector2.up * JumpForceUp * Time.deltaTime, ForceMode2D.Impulse);
        }

    }
    IEnumerator CoolDown() // just a coroutine to make a cooldown betwenn main jump and second jump
    {
        yield return new WaitForSeconds(CoolDownTimeBetweenJumps);
        if (!OnWall) CanDoubleJump = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && !OnWall)
        {
            if (collision.contacts[0].normal.y < 0) return; // Checks if the player landed on the wall from the  bottom, if so it does nothing

            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            OnWall = true;
            playerAnimator.SetBool("isJump", false);
            OnFloor = false;

            CurrentWall = collision.gameObject.GetComponent<Wall>();
            //Debug.Log(collision.gameObject.name);

            float Xdiference = transform.position.x - collision.transform.position.x;
            jumpSide = (sbyte)(Xdiference > 0 ? 1 : -1);
            transform.localScale = new Vector3(Xdiference > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
            ResetJump();

            CurrentWall.Touched(this);

            if (collision.contacts[0].normal.y > 0 && CurrentWall.FixIfOnTop) // Checks if the player landed on top of the wall, if  so it moves the player down a bit
            {
                CheckJumpSide();
                jumpSide *= -1;

                if (CurrentWall.OnlyLeftWall)
                {
                    jumpSide = -1;
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }
                if (CurrentWall.OnlyRightWall)
                {
                    jumpSide = 1;
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }
                StartCoroutine(BringPlayerOnPlatform(new Vector2(collision.transform.position.x + (transform.lossyScale.x * 0.5f), collision.contacts[0].point.y + 0.2f)));
            }
        }
        else if (collision.gameObject.CompareTag("Floor") && (!OnWall || (OnWall && CurrentWall.TypeOfWall == WallType.Lift)))
        {
            if (transform.position.y - collision.transform.position.y > 0) // Checks if player landed on floor from the top
            {
                OnFloor = true;

                CheckJumpSide();
                ResetJump();
            }

            if (collision.contacts[0].normal.y == 0) // Checks if player landed on floor from the side, if so it moves the player up a bit 
            {
                CheckJumpSide();
                StartCoroutine(BringPlayerOnPlatform(new Vector2(transform.position.x + 0.3f * jumpSide, collision.transform.position.y + 0.3f)));
            }
        }
    }

    public void CheckJumpSide() // Method to check the direction of the jump based on the player's velocity
    {
        if (LinearVelocityX > 0)
        {
            jumpSide = 1;

            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            jumpSide = -1;
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);

        }
    }

    IEnumerator BringPlayerOnPlatform(Vector2 targetPosition) // Method to bring the player on the platform when the player lands on it from the wrong side
    {
        rb.simulated = false; // disables the physics simulation for the player so it can be moved 
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10f;
            transform.position = Vector2.Lerp(transform.position, targetPosition, t);
            if (t > 0.8f) rb.simulated = true;
            yield return null;
        }
    }
    public void ResetJump() // Method to reset the jump when the player lands on the ground
    {
        CanJump = true;
        CanDoubleJump = false;
        playerAnimator.SetBool("isBackFlip", false);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && OnWall)
        {
            if (CurrentWall.TypeOfWall == WallType.Moving) return;


            rb.gravityScale = 2f;
            OnWall = false;
            CurrentWall.Left(this);
        }
        else if (collision.gameObject.CompareTag("Floor") && OnFloor)
        {
            OnFloor = false;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            Die();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (OnWall && collision.gameObject.CompareTag("Wall"))
        {
            CurrentWall.Staying(this);
        }
    }

    public void Die() // Method to reload the scene when the player dies
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void JumpEffect()
    {
        puffEffect.SetTrigger("Jump");
        //Debug.Log("JumpEffect");
    }
}