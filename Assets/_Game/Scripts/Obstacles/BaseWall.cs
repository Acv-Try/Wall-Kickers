//using UnityEngine;
//using System.Collections.Generic;

//public abstract class BaseWall : MonoBehaviour
//{
//    [Header("Wall Settings")]
//    [Tooltip("Speed of the player friction when sliding down the wall if its negative the player will slide up the wall")]
//    public float SpeedOfPlayerFriction = 0.5f;

//    public bool OnlyRightWall;
//    public bool OnlyLeftWall;
//    public bool FixIfOnTop = true;

//    public WallType TypeOfWall;

//    protected SoundData soundData;
//    protected void Initialize()
//    {
//        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Gameplay);
//    }

//    public virtual void Touched(PlayerController player){}
//    public virtual void Left(PlayerController player){}

//    public virtual void Staying(PlayerController player){}
//    }

using Unity.Cinemachine;
using UnityEngine;
public enum WallType
{
    Bounce, Electro, Default, Moving, Lift, Stone
}

public abstract class BaseWall : MonoBehaviour
{
    [Header("Wall Settings")]
    [TagField]
    [SerializeField] public string playerTag;
    public float SpeedOfPlayerFriction = 0.5f;
    public bool OnlyRightWall;
    public bool OnlyLeftWall;
    public bool FixIfOnTop = true;
    public WallType TypeOfWall;
    protected Animator _animator;
    protected SoundData soundData;
    protected string _playerTag => playerTag;
    protected void Initialize()
    {
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Gameplay);
        if(GetComponent<Animator>() != null)
            _animator = GetComponent<Animator>();
    }

    public virtual void Touched(PlayerController player) {}
    public virtual void Left(PlayerController player) {}
    public virtual void Staying(PlayerController player) { }

}