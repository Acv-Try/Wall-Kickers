using System.Collections;
using UnityEngine;

public class BounceWall : BaseWall
{
    [SerializeField] GameObject AnimationObject;
    [SerializeField] Transform HijhestPoint;
    [SerializeField] Transform LoverPoint;
    private void Start()
    {
        Initialize();
    }
    public override void Touched(PlayerController player)
    {
        AudioManager.Instance.Play(soundData,EType_Gameplay_SFX.Land_Wall_Bounce);
        player.CheckJumpSide();
        player.jumpSide *= -1;
        player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
        player.Jump();

        if(player.transform.position.y > HijhestPoint.position.y)
        {
          Animation(HijhestPoint.position.y);  
        }
        else if(player.transform.position.y < LoverPoint.position.y)
        {
             Animation(LoverPoint.position.y);  
        }
        else
        {
            Animation(player.transform.position.y);   
        }
    }
    public override void Left(PlayerController player)
    {

    }

    void Animation(float Ypos)
    { 
        GameObject AnimationObj = Instantiate(AnimationObject,new Vector2(transform.position.x, Ypos),Quaternion.identity);

        AnimationObj.GetComponent<Animator>().Play("BatutMid");
        Destroy(AnimationObj,1f);
    }
}
