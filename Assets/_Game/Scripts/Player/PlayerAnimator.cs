using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //[SerializeField] private GameObject puffEffectOB;
    [SerializeField] private PuffEffect puffEffect;
    [SerializeField] private ParticleSystem burstEffect;
    [SerializeField] private Vector3 puffEffectOffset;

    private Animator playerAnimator;
    private IPlayerController controller;
    private SpriteRenderer spriteRenderer;
    private SoundData soundData;
    private Coroutine coroutine;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        controller = GetComponent<IPlayerController>();
        Initialize();
    }
    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        PlayerManager.Instance.OnDeath -= OnDeath;
        PlayerManager.Instance.OnDeath += OnDeath;

        controller.OnJump -= OnJump;
        controller.OnJump += OnJump;

        controller.OnDoubleJump -= OnDoubleJump;
        controller.OnDoubleJump += OnDoubleJump;

        controller.OnWallTouched -= OnWallTouched;
        controller.OnWallTouched += OnWallTouched;

        controller.OnFloorTouched -= OnFloorTouched;
        controller.OnFloorTouched += OnFloorTouched;

        controller.OnFloorLeft -= OnFloorLeft;
        controller.OnFloorLeft += OnFloorLeft;
    }
    private void OnDestroy()
    {
    }
    private void Start()
    {
        //Initialize();
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Character);
        SetAnimations();
    }
    private void OnJump()
    {
        PlayJumpAudio();
        playerAnimator?.SetBool("isIdle", false);
        playerAnimator?.SetBool("isRunning", false);
        playerAnimator?.SetBool("isBackFlip", false);
        playerAnimator?.SetBool("isJump", true);
    }
    private void OnDoubleJump(sbyte side)
    {
        playerAnimator?.SetBool("isJump", false);
        playerAnimator?.SetBool("isBackFlip", true);
        PlayPuffEffect(side);
    }
    private void OnWallTouched()
    {
        playerAnimator?.SetBool("isJump", false);
        playerAnimator?.SetBool("isRunning", false);
        playerAnimator?.SetBool("isBackFlip", false);
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
        PlayDeathAudio();
        PlayBurstEffect();
        PlayDeathAudio();
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
    public void PlayBurstEffect()
    {
        burstEffect.Play();
    }

    public void SetAnimations()
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
}
