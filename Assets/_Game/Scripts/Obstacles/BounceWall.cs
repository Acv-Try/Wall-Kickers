using System.Collections;
using UnityEngine;

public class BounceWall : BaseWall
{
    [SerializeField] GameObject AnimationObject;
    [SerializeField] Transform HighestPoint;
    [SerializeField] Transform LowerPoint;
    private void Start()
    {
        Initialize();
    }
    public override void Touched(PlayerController player)
    {
        AudioManager.Instance.Play(soundData,EType_Gameplay_SFX.Land_Wall_Bounce);
        player.JumpFromBounce();


        if(player.transform.position.y > HighestPoint.position.y)
        {
          Animation(HighestPoint.position.y);  
        }
        else if(player.transform.position.y < LowerPoint.position.y)
        {
             Animation(LowerPoint.position.y);  
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
