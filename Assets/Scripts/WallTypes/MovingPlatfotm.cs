//using System.Collections;
//using UnityEngine;
//public class MovingPlatform : Wall
//{
//    Rigidbody2D rb;
//    BoxCollider2D col;

//    void Start()
//    {
//        col = GetComponent<BoxCollider2D>();
//    }
//    public override void Touched(Player player)
//    {
//        player.transform.SetParent(transform);
//        rb = player.rb;
//    }

//    public override void Left(Player player)
//    {
//        player.transform.SetParent(null);
//        rb = null;
//        col.enabled = false;
//        StartCoroutine(ResetColider());
//    }

//    IEnumerator ResetColider()
//    {
//        yield return new WaitForSeconds(0.1f);
//         col.enabled = true;
//    }

//    void FixedUpdate()
//    {
//         PlatformVelocity = ((Vector2)transform.position - LastPosition) / Time.fixedDeltaTime;

//        LastPosition = transform.position;

//        if(rb != null)
//        {
//            rb.position += PlatformVelocity * Time.fixedDeltaTime;
//            rb.linearVelocity = Vector2.zero;
//        }
//    }

//    Vector2 LastPosition;
//    Vector2 PlatformVelocity;

//}