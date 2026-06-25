using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator puffEffect;
    [SerializeField] private ParticleSystem burstEffect;
    [SerializeField] private Vector3 puffEffectOffset;

    private PlayerController controller;
    private SpriteRenderer spriteRenderer;
    private SoundData soundData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        controller.OnWallTouched += OnWallTouched;
        controller.OnWallLeft += OnWallLeft;
        controller.OnDoubleJumpPerformed += OnDoubleJump;
    }

    private void OnDestroy()
    {
        controller.OnWallTouched -= OnWallTouched;
        controller.OnWallLeft -= OnWallLeft;
        controller.OnDoubleJumpPerformed -= OnDoubleJump;
    }
    private void Start()
    {
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Character);
    }
    private void OnWallTouched(BaseWall wall)
    {
        playerAnimator.SetBool("isJump", false);
    }

    private void OnWallLeft(BaseWall wall)
    {
        playerAnimator.SetBool("isJump", true);
        PlayPuffEffect(false);
    }

    private void OnDoubleJump()
    {
        playerAnimator.SetBool("isBackFlip", true);
        PlayPuffEffect(true);
        PlayJumpAudio();
    }

    public void PlayPuffEffect(bool isReversed)
    {
        var instance = Instantiate(
            puffEffect,
            transform.position + puffEffectOffset,
            Quaternion.identity
        );
        instance.transform.localScale = new Vector3(
            1,
            controller.JumpSide * (isReversed ? -1 : 1),
            1
        );
        instance.SetTrigger("Jump");
    }

    public void PlayBurstEffect()
    {
        burstEffect.Play();
    }

    public void ResetAnimations()
    {
        playerAnimator.SetBool("isJump", false);
        playerAnimator.SetBool("isBackFlip", false);
    }

    public void SetVisible(bool visible)
    {
        spriteRenderer.enabled = visible;
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
