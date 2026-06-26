using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{

    [Header("Jump Settings")]
    public float JumpForceUp = 15f;
    public float StartJumpForceUp = 7f;
    public float JumpForceSide = 3f;
    public float JumpTime = 0.5f;
    public float CoolDownTimeBetweenJumps = 0.2f;
    public float GravityForceWhileFalling = 3f;
    public float SpeedOnFloor = 5f;

    public event Action<BaseWall> OnWallTouched;
    public event Action<BaseWall> OnWallLeft;
    public event Action OnDoubleJumpPerformed;

    public sbyte JumpSide { get; private set; } = 1;
    public bool IsOnWall { get; private set; }
    public bool IsOnFloor { get; private set; }
    public bool CanJump { get; private set; }
    public bool CanDoubleJump { get; private set; }
    public BaseWall CurrentWall { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    private PlayerInput input;
    private float jumpTimeCounter;
    private sbyte linearVelocityX;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();

        input.OnTouchBegan += HandleTouchBegan;
        input.OnTouchHeld += HandleTouchHeld;
    }

    private void OnDestroy()
    {
        input.OnTouchBegan -= HandleTouchBegan;
        input.OnTouchHeld -= HandleTouchHeld;
    }

    public void Initialize()
    {
        CanJump = true;
        CanDoubleJump = false;
        IsOnWall = true;
        IsOnFloor = false;
        JumpSide = 1;
        jumpTimeCounter = 0f;
        Rb.gravityScale = 2f;
        Rb.linearVelocity = Vector2.zero;
    }
    public void JumpFromBounce()
    {
        CheckJumpSide();
        JumpSide *= -1;
        FlipScale(JumpSide);
        Jump();
    }
    private void HandleTouchBegan()
    {
        if (CanJump)
        {
            Jump();
            return;
        }

            if (CanDoubleJump)
            {
                AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Jump);
                playerAnimator.SetBool("isBackFlip", true);
                JumpEffect(true);
                StartJump((sbyte)-jumpSide);
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                CanDoubleJump = false;
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && !OnWall)
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
        CanJump = false;
        IsOnWall = false;
        StartCoroutine(CoolDown());
        ApplyJumpForce(JumpSide);

        if (CurrentWall != null)// && CurrentWall.TypeOfWall == WallType.Moving)
        {
            CurrentWall.Left(this);
            Rb.gravityScale = 2f;
            IsOnWall = false;
        }
    }

    private void DoubleJump()
    {
        CanDoubleJump = false;
        sbyte doubleSide = (sbyte)-JumpSide;
        ApplyJumpForce(doubleSide);
        FlipScale(doubleSide);
        OnDoubleJumpPerformed?.Invoke();
    }

    private void ApplyJumpForce(sbyte side)
    {
        Rb.linearVelocity = Vector2.zero;
        Rb.gravityScale = 2f;
        Rb.AddForce(new Vector2(JumpForceSide * side, StartJumpForceUp), ForceMode2D.Impulse);
        jumpTimeCounter = 0f;
        linearVelocityX = (sbyte)Rb.linearVelocity.x;
    }

    private void VerticalJump()
    {
        jumpTimeCounter += Time.deltaTime;
        if (jumpTimeCounter < JumpTime)
            Rb.AddForce(Vector2.up * JumpForceUp * Time.deltaTime, ForceMode2D.Impulse);
    }

    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    private void FixedUpdate()
    {
        if (IsOnFloor)
            Rb.linearVelocity = new Vector2(SpeedOnFloor * JumpSide * Time.deltaTime, Rb.linearVelocity.y);

        if (IsOnWall && CurrentWall != null)
            Rb.position += Vector2.down * CurrentWall.SpeedOfPlayerFriction * Time.deltaTime;

        if (Rb.linearVelocity.y < -0.2f && !IsOnWall)
            Rb.gravityScale = GravityForceWhileFalling;
    }
    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(CoolDownTimeBetweenJumps);
        if (!IsOnWall) CanDoubleJump = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall") && !IsOnWall)
        {
            Debug.Log("Enter");
            if (collision.contacts[0].normal.y < 0)
            {
                jumpTimeCounter = JumpTime;
                return;
            }

            Rb.gravityScale = 0f;
            Rb.linearVelocity = Vector2.zero;
            IsOnWall = true;
            IsOnFloor = false;

            CurrentWall = collision.gameObject.GetComponent<BaseWall>();
            float xDiff = transform.position.x - collision.transform.position.x;
            JumpSide = (sbyte)(xDiff > 0 ? 1 : -1);

            ResetJump();
            CurrentWall.Touched(this);
            OnWallTouched?.Invoke(CurrentWall);

            if (collision.contacts[0].normal.y > 0 && CurrentWall.FixIfOnTop)
            {
                JumpSide *= -1;
                if (CurrentWall.OnlyLeftWall) JumpSide = -1;
                if (CurrentWall.OnlyRightWall) JumpSide = 1;
                StartCoroutine(BringPlayerOnPlatform(new Vector2(
                    collision.transform.position.x + (-JumpSide * Math.Abs(transform.lossyScale.x) * 0.5f),
                    transform.position.y - 0.3f
                )));
                CheckJumpSide();
            }

            FlipScale(JumpSide);
        }
        else if (collision.gameObject.CompareTag("Floor") && (!OnWall || (OnWall && CurrentWall.TypeOfWall == WallType.Lift)))
        {
            if (transform.position.y - collision.transform.position.y > 0) // Checks if player landed on floor from the top
            {
                IsOnFloor = true;
                CheckJumpSide();
                ResetJump();
            }

            if (collision.contacts[0].normal.y == 0)
            {
                CheckJumpSide();
                StartCoroutine(BringPlayerOnPlatform(new Vector2(
                    transform.position.x + 0.3f * JumpSide,
                    collision.transform.position.y + 0.3f
                )));
            }
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && IsOnWall)
        {
            if (CurrentWall != null && CurrentWall.TypeOfWall == WallType.Moving) return;
            Rb.gravityScale = 2f;
            IsOnWall = false;
            CurrentWall.Left(this);
            OnWallLeft?.Invoke(CurrentWall);
            CurrentWall = null;
        }
        else if (collision.gameObject.CompareTag("Floor") && IsOnFloor)
        {
            IsOnFloor = false;
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (IsOnWall && collision.gameObject.CompareTag("Wall") && CurrentWall != null)
            CurrentWall.Staying(this);
    }

    public void ResetJump()
    {
        CanJump = true;
        CanDoubleJump = false;
    }

    public void CheckJumpSide()
    {
        JumpSide = (sbyte)(linearVelocityX > 0 ? 1 : -1);
    }

    public void FlipScale(sbyte side)
    {
        transform.localScale = new Vector3(side, transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator BringPlayerOnPlatform(Vector2 targetPosition)
    {
        Rb.simulated = false;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10f;
            transform.position = Vector2.Lerp(transform.position, targetPosition, t);
            if (t > 0.8f) Rb.simulated = true;
            yield return null;
        }
    }

}
//[Header("Jump Settings")]

//[Tooltip("Force that applies every frame while the jump button is held down")]
//public float JumpForceUp = 15f;

//[Tooltip("Starting force applied when the jump button is first pressed")]
//public float StartJumpForceUp = 7;

//[Tooltip("Force applied to the side when the jump button is first pressed")]
//public float JumpForceSide = 3f;

//[Tooltip("Maximum time the jump button can be held")]
//public float JumpTime = 0.5f;

//[Tooltip("Time after a jump before the player can make second jump")]
//public float CoolDownTimeBetweenJumps = 0.2f;

//[Tooltip("Gravity scale applied when the player is falling")]
//public float GravityForceWhileFalling = 3;

//[Tooltip("Speed of the player when on the floor")]
//public float SpeedOnFloor = 5f;

//[SerializeField] private Animator playerAnimator;
//[SerializeField] private Animator puffEffect;
//[SerializeField] private ParticleSystem burstEffect;
//[SerializeField] private Vector3 puffEffectOffset;

//public sbyte jumpSide = 1; // a variable to determine the direction of the jump, 1 for right and -1 for left. It is set when the player collides with a wall.
//[SerializeField]
//private bool CanJump = true;
//[SerializeField]
//private bool OnWall = false;
//[SerializeField]
//private bool OnFloor = false;
//[SerializeField]
//private bool CanDoubleJump = false;
//[SerializeField]
//private sbyte LinearVelocityX;

//public Transform spawnPos;
//public SpriteRenderer playerSprite { get; set; }

//public Action<Vector3> OnCameraCheckPointChange;
//public Action OnCameraFreeze;

//public event Action OnCamerShake;
//public float lastCheckpointY { get; set; }
//public int checkPoint;
//public int checkPointCount;
//public bool isCameraMoving;
//public CheckPointWall currentCheckPointWall;

//public bool isDead { get; set; }

//float JumpTimeCounter;
//public Rigidbody2D rb;

//public BaseWall CurrentWall;
//private Vector3 levelCenter;
//private SoundData soundData;


//public void Initialize()
//{
//    levelCenter = GridGenerator.Instance.GetFirstLevelCenter();
//    Vector3 spawnPos = GridGenerator.Instance.GetSpawnPosition();
//    OnCameraCheckPointChange?.Invoke(levelCenter);
//    soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Character);
//    transform.position = spawnPos;
//    rb = GetComponent<Rigidbody2D>();
//    playerSprite = GetComponent<SpriteRenderer>();
//    lastCheckpointY = transform.position.y;
//}

//void Update()
//{
//    if (isCameraMoving) return;
//    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
//    {
//        if (CanJump)
//        {
//            AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Jump);
//            AudioManager.Instance.PlayAndTrack(soundData, EType_Gameplay_SFX.C_Monkey_JumpEffect);
//            JumpEffect();
//            Jump();
//        }

//        if (CanDoubleJump)
//        {
//            AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Jump);
//            playerAnimator.SetBool("isBackFlip", true);
//            JumpEffect(true);
//            StartJump((sbyte)-jumpSide);
//            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
//            CanDoubleJump = false;
//        }
//    }

//    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && !OnWall)
//    {
//        VerticalJump();
//    }

//    if (rb.linearVelocity.y < -0.2f && !OnWall) // makes a gravity stronger then player is falling 
//    {
//        rb.gravityScale = GravityForceWhileFalling;
//    }

//    if (OnWall) // applies friction to the player when on the wall so the player is slides down from the wall
//    {
//        rb.position += Vector2.down * CurrentWall.SpeedOfPlayerFriction * Time.deltaTime;
//    }
//}

//void FixedUpdate()
//{
//    if (OnFloor)
//    {
//        rb.linearVelocity = new Vector2(SpeedOnFloor * jumpSide * Time.deltaTime, rb.linearVelocity.y);
//    }
//}

//public void Jump()
//{
//    playerAnimator.SetBool("isJump", true);
//    StartJump(jumpSide);
//    CanJump = false;
//    if (CurrentWall.TypeOfWall == WallType.Moving)
//    {
//        CurrentWall.Left(this);
//        rb.gravityScale = 2f;
//        OnWall = false;
//    }
//    StartCoroutine(CoolDown());

//}

//void StartJump(sbyte side) // Method to start the jump
//{
//    rb.linearVelocity = Vector2.zero;
//    rb.gravityScale = 2f;

//    rb.AddForce(new Vector2(JumpForceSide * side, StartJumpForceUp), ForceMode2D.Impulse);
//    JumpTimeCounter = 0;

//    LinearVelocityX = (sbyte)rb.linearVelocity.x;
//}

//void VerticalJump() // Method for the vertical part of the jump that applies force while the jump button pressed
//{
//    JumpTimeCounter += Time.deltaTime;

//    if (JumpTimeCounter < JumpTime)
//    {
//        rb.AddForce(Vector2.up * JumpForceUp * Time.deltaTime, ForceMode2D.Impulse);
//    }

//}
//IEnumerator CoolDown() // just a coroutine to make a cooldown betwenn main jump and second jump
//{
//    yield return new WaitForSeconds(CoolDownTimeBetweenJumps);
//    if (!OnWall) CanDoubleJump = true;
//}

//void OnCollisionEnter2D(Collision2D collision)
//{
//    if (collision.gameObject.CompareTag("Wall") && !OnWall)
//    {
//        AudioManager.Instance.Stop(EType_Gameplay_SFX.C_Monkey_JumpEffect);
//        //AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.Land_Wall_Wood);
//        if (collision.contacts[0].normal.y < 0)
//        {
//            JumpTimeCounter = JumpTime;
//            return;
//        } // Checks if the player landed on the wall from the  bottom, if so it does nothing

//        rb.gravityScale = 0f;
//        rb.linearVelocity = Vector2.zero;
//        OnWall = true;
//        if (currentCheckPointWall != null)
//        {
//            IncreaseCheckPoint();
//            currentCheckPointWall = null;
//        }
//        playerAnimator.SetBool("isJump", false);
//        OnFloor = false;

//        CurrentWall = collision.gameObject.GetComponent<BaseWall>();
//        float XDifference = transform.position.x - collision.transform.position.x;
//        jumpSide = (sbyte)(XDifference > 0 ? 1 : -1);

//        ResetJump();

//        CurrentWall.Touched(this);

//        if (collision.contacts[0].normal.y > 0 && CurrentWall.FixIfOnTop) // Checks if the player landed on top of the wall, if  so it moves the player down a bit
//        {
//            jumpSide *= -1;

//            if (CurrentWall.OnlyLeftWall) jumpSide = -1;
//            if (CurrentWall.OnlyRightWall) jumpSide = 1;

//            StartCoroutine(BringPlayerOnPlatform(new Vector2(collision.transform.position.x + (-jumpSide * Math.Abs(transform.lossyScale.x) * 0.5f), transform.position.y - 0.3f)));
//            CheckJumpSide();
//        }

//        transform.localScale = new Vector3(jumpSide, transform.localScale.y, transform.localScale.z);
//    }
//    else if (collision.gameObject.CompareTag("Floor") && (!OnWall || (OnWall && CurrentWall.TypeOfWall == WallType.Lift)))
//    {
//        if (transform.position.y - collision.transform.position.y > 0) // Checks if player landed on floor from the top
//        {
//            OnFloor = true;

//            CheckJumpSide();
//            ResetJump();
//        }

//        if (collision.contacts[0].normal.y == 0) // Checks if player landed on floor from the side, if so it moves the player up a bit 
//        {
//            CheckJumpSide();
//            StartCoroutine(BringPlayerOnPlatform(new Vector2(transform.position.x + 0.3f * jumpSide, collision.transform.position.y + 0.3f)));
//        }
//    }
//}

//public void CheckJumpSide() // Method to check the direction of the jump based on the player's velocity
//{
//    if (LinearVelocityX > 0)
//    {
//        jumpSide = 1;

//    }
//    else
//    {
//        jumpSide = -1;

//    }
//}

//IEnumerator BringPlayerOnPlatform(Vector2 targetPosition) // Method to bring the player on the platform when the player lands on it from the wrong side
//{
//    rb.simulated = false; // disables the physics simulation for the player so it can be moved 
//    float t = 0;
//    while (t < 1)
//    {
//        t += Time.deltaTime * 10f;
//        transform.position = Vector2.Lerp(transform.position, targetPosition, t);
//        if (t > 0.8f) rb.simulated = true;
//        yield return null;
//    }
//}
//public void ResetJump() // Method to reset the jump when the player lands on the ground
//{
//    CanJump = true;
//    CanDoubleJump = false;
//    playerAnimator.SetBool("isBackFlip", false);
//}

//void OnCollisionExit2D(Collision2D collision)
//{
//    if (collision.gameObject.CompareTag("Wall") && OnWall)
//    {
//        if (CurrentWall.TypeOfWall == WallType.Moving) return;


//        rb.gravityScale = 2f;
//        OnWall = false;
//        CurrentWall.Left(this);
//    }
//    else if (collision.gameObject.CompareTag("Floor") && OnFloor)
//    {
//        OnFloor = false;
//    }
//}
//void OnTriggerEnter2D(Collider2D collision)
//{
//    if (collision.gameObject.CompareTag("Dead"))
//    {
//        AudioManager.Instance.Stop(EType_Gameplay_SFX.C_Monkey_JumpEffect);
//        AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Death);
//        AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Death_Explosion);

//        playerSprite.enabled = false;
//        rb.simulated = false;
//        burstEffect.Play();
//        OnCamerShake?.Invoke();
//        Die();
//    }
//}

//void OnCollisionStay2D(Collision2D collision)
//{
//    if (OnWall && collision.gameObject.CompareTag("Wall"))
//    {
//        CurrentWall.Staying(this);
//    }
//}

//public void Die() // Method to reload the scene when the player dies
//{
//    isDead = true;
//    if (checkPoint < 20)
//    {
//        OnCameraCheckPointChange?.Invoke(levelCenter);
//        return;
//    }
//    StartCoroutine(LoadSceneAfterCoolDown(1f));
//}

//public void JumpEffect(bool isReversed = false)
//{

//    var puffEffectInstance = Instantiate(puffEffect, transform.position + puffEffectOffset, Quaternion.identity);
//    puffEffectInstance.transform.localScale = new Vector3(1, jumpSide * (isReversed ? -1 : 1), 1);
//    puffEffectInstance.SetTrigger("Jump");
//    //Debug.Log("JumpEffect");
//}

//public void ResetPlayerStats()
//{
//    checkPoint = 0;
//    checkPointCount = 0;
//    lastCheckpointY = 0;
//    GridGenerator.Instance.AddScore(0);
//}

//public IEnumerator LoadSceneAfterCoolDown(float duration)
//{
//    OnCameraFreeze?.Invoke();
//    yield return new WaitForSeconds(duration);
//    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

//}

//public void IncreaseCheckPoint()
//{
//    checkPoint++;
//    checkPointCount++;
//    if (checkPointCount == 10) checkPointCount = 0;
//    GridGenerator.Instance.CheckIfPlayerAboveOfMiddleLevel(transform.position, checkPoint);
//}