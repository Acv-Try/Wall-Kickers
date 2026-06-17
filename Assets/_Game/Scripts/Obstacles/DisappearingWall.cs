//using UnityEngine;
//using System.Collections;
//using TMPro;


//public class DisappearingWall : BaseWall
//{
//   public float TimeToRespawn = 4f;
  
//  Collider2D col;
//    SpriteRenderer sr;

//    void Start()
//    {
//         col = GetComponent<Collider2D>();
//        sr = GetComponent<SpriteRenderer>();
//    }
//    public override void Left(Player player)
//    {
//        StartCoroutine(Timer());
//    }
   
//    void ChangeState(bool state)
//    {
//        col.enabled = state;
//        sr.enabled = state;
//    }

//    IEnumerator Timer()
//    {
//        ChangeState(false);

//        yield return new WaitForSeconds(TimeToRespawn);

//        ChangeState(true);
//    }
//}
