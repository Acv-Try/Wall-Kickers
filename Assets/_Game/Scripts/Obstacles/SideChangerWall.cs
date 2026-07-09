//using System.Collections;
//using TMPro;
//using UnityEngine;
//public class SideChangerWall : BaseWall
//{
//   public float TimeBetweenSideChange = 4f;
//    PlayerController Player;
//    [SerializeField] TMP_Text TimerText;
//    public override void Touched(PlayerController player)
//    {
//        Player = player;
//    }
//    public override void Left(PlayerController player)
//    {
//        Player = null;
//    }
//    public void Start()
//    {
//        StartCoroutine(Timer());
//    }
//    void ChangeSide()
//    {
//        transform.localScale *= new Vector2(-1,1);
//        if(Player != null)
//        {
//        Vector3 offset = Player.transform.position - transform.position;
//        Player.rb.position = transform.position + new Vector3(-offset.x,offset.y,offset.z);
//        Player.jumpSide *= -1;
//        }
//    }
//    IEnumerator Timer()
//    {
//        while(true)
//        {
//        float Timer = TimeBetweenSideChange;
//        TimerText.text = Timer.ToString();
//          while(Timer > 0)
//            {
//                 yield return new WaitForSeconds(1);
//                 Timer--;
//                 TimerText.text = Timer.ToString();
//            }          
//        ChangeSide();
//        }
//    }
//}