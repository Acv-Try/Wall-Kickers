using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
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


 [SerializeField] private sbyte jumpSide = 1; // a variable to determine the direction of the jump, 1 for right and -1 for left. It is set when the player collides with a wall.
 [SerializeField] private bool CanJump = true;
 [SerializeField] private bool OnWall = true;
 [SerializeField] private bool OnFloor = false;
 [SerializeField] private bool CanDoubleJump = false;
 [SerializeField] private sbyte LinearVelocityX;

 [Header("Camera Settings")]
 [SerializeField] CinemachineFollow cam;

  float JumpTimeCounter;
  Rigidbody2D rb;

  Wall CurrentWall;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
                if(CanJump)            
                {
                  StartJump(jumpSide);
                  CanJump = false;
                  StartCoroutine(CoolDown());
                }

                if(CanDoubleJump)
                {
                  StartJump((sbyte)-jumpSide);
                  CanDoubleJump = false;
                }
        }

        if(Input.GetKey(KeyCode.Space) && !OnWall)
        {
           VerticalJump();
        }

        if(rb.linearVelocity.y < -0.2f) // makes a gravity stronger then player is falling 
        {
            rb.gravityScale = GravityForceWhileFalling;
        }

        if(OnWall) // applies friction to the player when on the wall so the player is slides down from the wall
        {
            rb.position += Vector2.down * CurrentWall.SpeedOfPlayerFriction * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if(OnFloor)
        {
            rb.linearVelocity = new Vector2(SpeedOnFloor * jumpSide * Time.deltaTime, rb.linearVelocity.y);
        }
    }

    void StartJump(sbyte side) //Method to start the jump
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
            
            if(JumpTimeCounter < JumpTime)
            {
                rb.AddForce(Vector2.up * JumpForceUp * Time.deltaTime, ForceMode2D.Impulse);
            }          

    }
     IEnumerator CoolDown() // just a coroutine to make a cooldown betwenn main jump and second jump
    {
        yield return new WaitForSeconds(CoolDownTimeBetweenJumps);
        if(!OnWall) CanDoubleJump = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {         
        if(collision.gameObject.CompareTag("Wall"))
        {
          rb.gravityScale = 0f; 
          rb.linearVelocity = Vector2.zero;
          OnWall = true;
          OnFloor = false;
           CurrentWall = collision.gameObject.GetComponent<Wall>();

          
          float Xdiference = transform.position.x - collision.transform.position.x;
          if(collision.contacts[0].normal.y > 0) // Checks if the player landed on top of the wall, if  so it moves the player down a bit
           {
               int side = 0;
               if(CurrentWall.OnlyLeftWall  || (Xdiference > 0 && !CurrentWall.OnlyLeftWall && !CurrentWall.OnlyRightWall)) side = -1;
               if(CurrentWall.OnlyRightWall || (Xdiference < 0 && !CurrentWall.OnlyLeftWall && !CurrentWall.OnlyRightWall)) side = 1;

               StartCoroutine(BringPlayerOnPlatform(new Vector2(collision.transform.position.x + side * 0.4f, transform.position.y - 0.2f)));
           }    

           jumpSide = (sbyte)(Xdiference > 0 ? 1 : -1) ;         

           ResetJump();
        }
        else if(collision.gameObject.CompareTag("Floor") && !OnWall)
        {
            if(transform.position.y - collision.transform.position.y > 0) // Checks if player landed on floor from the top
            {
                OnFloor = true;

                CheckJumpSide();
                ResetJump();
            }

            if(collision.contacts[0].normal.y == 0) // Checks if player landed on floor from the side, if so it moves the player up a bit 
            {
                CheckJumpSide();
                StartCoroutine(BringPlayerOnPlatform(new Vector2(transform.position.x + 0.4f * jumpSide, collision.transform.position.y + 0.3f)));
            }
        }
    }

    void CheckJumpSide() // Method to check the direction of the jump based on the player's velocity
    {
        if(LinearVelocityX > 0)
        {
            jumpSide = 1;
        }
        else
        {
            jumpSide = -1;
        }
    }

    IEnumerator BringPlayerOnPlatform(Vector2 targetPosition) // Method to bring the player on the platform when the player lands on it from the wrong side
    {
        rb.simulated = false; // disables the physics simulation for the player so it can be moved 
        float t = 0;
            while(t < 1)
            {
                t += Time.deltaTime * 4f;
                transform.position = Vector2.Lerp(transform.position, targetPosition, t);
                yield return null;
            }
         rb.simulated = true;
    }
    void ResetJump() // Method to reset the jump when the player lands on the ground
    {
        CanJump = true;
        CanDoubleJump = false;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && OnWall)
        {
          rb.gravityScale = 2f;
          OnWall = false;
        }
        else if(collision.gameObject.CompareTag("Floor") && OnFloor)
        {
            OnFloor = false;
        }
    }
}
