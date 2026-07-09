using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
public class MovingPlatform1 : BaseWall
{


    public override void Touched(PlayerController player)
    {
        player.transform.SetParent(transform);
    }

    public override void Left(PlayerController player)
    {
        player.transform.SetParent(null);
    }

}