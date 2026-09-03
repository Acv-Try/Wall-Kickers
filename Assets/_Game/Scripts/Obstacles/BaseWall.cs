
using System;
using UnityEngine;
public enum WallType
{
    Default,
    Bounce,
    Electro,
    Moving,
    Lift,
    Disappearing
}

public abstract class BaseWall : MonoBehaviour
{
    [Header("Wall Settings")]
    [SerializeField] public float PlayersFrictionSpeed = 0.5f;
    [SerializeField] protected bool onlyRightWall;
    [SerializeField] protected bool onlyLeftWall;
    [SerializeField] protected bool fixIfOnTop = true;
    [SerializeField] protected WallType wallType;

    protected Animator _animator;
    protected SoundData soundData;


    public WallType Type => wallType;
    public bool FixIfOnTop => fixIfOnTop;
    public bool OnlyRightWall => onlyRightWall;
    public bool OnlyLeftWall => onlyLeftWall;

    public event Action<PlayerController> OnPlayerTouched;
    public event Action<PlayerController> OnPlayerLeft;


    protected void Initialize()
    {
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Gameplay);
        if (GetComponent<Animator>() != null)
            _animator = GetComponent<Animator>();
    }

    public virtual void Touched(PlayerController player)
    {
        Debug.Log("True");
        OnPlayerTouched?.Invoke(player);
        
    }
    public virtual void Left(PlayerController player)
    {
        Debug.Log("False");
        OnPlayerLeft?.Invoke(player);
    }
    public virtual void Staying(PlayerController player)
    {
    }
    protected void PlayLandAudio(EType_Gameplay_SFX type)
    {
        AudioManager.Instance.Play(soundData, type);
    }
}