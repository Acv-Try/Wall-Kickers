using UnityEngine;

public class BatutWall : Wall
{
    [SerializeField] private Animator wallAnimator;
    public override void Touched(Player player)
    {
        player.CheckJumpSide();
        player.jumpSide *= -1;
        player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
        player.Jump();
    }
    public override void Left(Player player)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        wallAnimator.SetTrigger("isTouch");
    }
}
