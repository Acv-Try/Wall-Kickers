using UnityEngine;
using System.Collections.Generic;

public enum WallType
{
    Bounce,Electro,Default,Moving,Lift
}
public abstract class BaseWall : MonoBehaviour
{
    [Header("Wall Settings")]
    [Tooltip("Speed of the player friction when sliding down the wall if its negative the player will slide up the wall")]
    public float SpeedOfPlayerFriction = 0.5f;

    public bool OnlyRightWall;
    public bool OnlyLeftWall;
    public bool FixIfOnTop = true;

    public WallType TypeOfWall;

    public virtual void Touched(PlayerController player){}
    public virtual void Left(PlayerController player){}

    public virtual void Staying(PlayerController player){}
    }

