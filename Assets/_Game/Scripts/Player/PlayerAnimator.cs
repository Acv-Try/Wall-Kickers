using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //[SerializeField] private GameObject puffEffectOB;
    [SerializeField] private PuffEffect puffEffect;
    [SerializeField] private ParticleSystem burstEffect;
    [SerializeField] private Vector3 puffEffectOffset;

    private Animator playerAnimator;
    private IPlayerStatus status;
    private IPlayerController controller;
    private SpriteRenderer spriteRenderer;
    private SoundData soundData;
    private Coroutine coroutine;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        status = GetComponent<IPlayerStatus>();
        controller = GetComponent<IPlayerController>();
        Initialize();
    }
    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        status.OnDeath += OnDeath;
        status.OnRespawn += OnRespawn;
        controller.OnJump += OnJump;
        controller.OnDoubleJump += OnDoubleJump;
        controller.OnWallTouched += OnWallTouched;
        controller.OnFloorTouched += OnFloorTouched;
        controller.OnFloorLeft += OnFloorLeft;

    }


    private void OnDestroy()
    {
        status.OnDeath -= OnDeath;
        status.OnRespawn -= OnRespawn;
        controller.OnJump -= OnJump;
        controller.OnDoubleJump -= OnDoubleJump;
        controller.OnWallTouched -= OnWallTouched;
        controller.OnFloorTouched -= OnFloorTouched;
        controller.OnFloorLeft -= OnFloorLeft;
    }
    private void Start()
    {
        //Initialize();
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Character);
        ResetAnimations();
    }
    private void OnJump()
    {
        playerAnimator?.SetBool("isIdle", false);
        playerAnimator?.SetBool("isBackFlip", false);
        playerAnimator?.SetBool("isJump", true);
    }
    private void OnDoubleJump(sbyte side)
    {
        playerAnimator?.SetBool("isJump", false);
        playerAnimator?.SetBool("isBackFlip", true);
        //PlayPuffEffect(side);
    }
    private void OnWallTouched()
    {
        playerAnimator?.SetBool("isBackFlip", false);
        playerAnimator?.SetBool("isJump", false);
        playerAnimator?.SetBool("isRunning", false);
        playerAnimator?.SetBool("isIdle", true);

    }

    private void OnFloorTouched()
    {
        playerAnimator?.SetBool("isBackFlip", false);
        playerAnimator?.SetBool("isJump", false);
        playerAnimator?.SetBool("isRunning", true);
    }
    private void OnFloorLeft()
    {
        playerAnimator?.SetBool("isRunning", false);
        playerAnimator?.SetBool("isJump", true);
    }
    private void OnDeath()
    {
        PlayBurstEffect();
        PlayDeathAudio();
        spriteRenderer.enabled = false;
    }
    private void OnRespawn()
    {
        spriteRenderer.enabled = true;
        ResetAnimations();
    }

    public void PlayPuffEffect(sbyte side)
    {
        PuffEffect instance = Instantiate(
            puffEffect,
            transform.position + puffEffectOffset,
            Quaternion.identity
        );
        instance.transform.localScale = new Vector3(
            side,
            instance.transform.localScale.y,
            instance.transform.localScale.z
        );
    }
    //if(coroutine != null)
    //{
    //    coroutine = null;
    //    coroutine = StartCoroutine(PuffEffectLifeCycle(puffEffect));
    //}

    public void PlayBurstEffect()
    {
        burstEffect.Play();
    }

    public void ResetAnimations()
    {
        playerAnimator?.SetBool("isJump", false);
        playerAnimator?.SetBool("isBackFlip", false);
        playerAnimator?.SetBool("isRunning", false);
        playerAnimator?.SetBool("isIdle", true);
    }
    private void PlayJumpAudio()
    {
        AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Jump);
        AudioManager.Instance.PlayAndTrack(soundData, EType_Gameplay_SFX.C_Monkey_JumpEffect);
    }
    public void PlayDeathAudio()
    {
        AudioManager.Instance.Stop(EType_Gameplay_SFX.C_Monkey_JumpEffect);
        AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Death);
        AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.C_Monkey_Death_Explosion);
    }
    //IEnumerator PuffEffectLifeCycle(GameObject puffEffect)
    //{
    //    Debug.Log("---- playing puff effect");
    //    Animator animatorPuffEffect = puffEffect.GetComponent<Animator>();
    //    animatorPuffEffect.SetTrigger("Jump");
    //    yield return new WaitForSeconds(2f);
    //    Destroy(animatorPuffEffect);
    //    Destroy(puffEffectOB);
    //}
}
