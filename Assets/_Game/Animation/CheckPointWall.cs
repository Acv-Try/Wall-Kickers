//using System;
//using TMPro;
//using UnityEngine;
//
//public class CheckPointWall : BaseWall
//{
//    [SerializeField] TextMeshPro checkPointText;
//    [SerializeField] Animator WallAnimator;
//    public override void Touched(PlayerController player)
//    {
//        
//            var playerScript = player;
//            playerScript.currentCheckPointWall = null;
//            playerScript.IncreaseCheckPoint();
//            AnimationPlay();
//        
//    }
//
//    public void OnTriggerEnter2D(Collider2D collider)
//    {
//        if (collider.CompareTag("Player"))
//        {
//            var playerScript = collider.GetComponent<PlayerController>();
//            playerScript.currentCheckPointWall = null;
//            if (Convert.ToInt16(checkPointText.text) <= playerScript.checkPoint) return;
//             AnimationPlay();
//            playerScript.currentCheckPointWall = this;
//        } 
//    }
//
//   public override void Left(PlayerController player)
//    {
//        if (player.rb.linearVelocityY < 0)
//        {
//            var playerScript = player;
//            playerScript.currentCheckPointWall = null;
//        }
//    }
//
//    public void SetCheckPointText(int checkPoint)
//    {
//        checkPointText.text = checkPoint.ToString();
//    }
//
//    void AnimationPlay()
//    {
//        WallAnimator.Play("StartAnimation");
//    }
//}
//