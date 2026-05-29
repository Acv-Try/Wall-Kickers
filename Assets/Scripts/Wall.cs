using UnityEngine;

public class Wall: MonoBehaviour
{
    [Header("Wall Settings")]
    [Tooltip("Speed of the player friction when sliding down the wall if its negative the player will slide up the wall")]
    public float SpeedOfPlayerFriction = 0.5f;

    public bool OnlyRightWall;
    public bool OnlyLeftWall;
}
