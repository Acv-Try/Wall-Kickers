////using UnityEngine;
//
////public class CheckPoint : MonoBehaviour
////{
////    public int checkPointNum;
////    private void OnTriggerEnter2D(Collider2D collider)
////    {
////        if (collider.CompareTag("Player"))
////        {
////            var scriptPlayer = collider.GetComponent<PlayerController>();
////            if (scriptPlayer.checkPointCount >= checkPointNum) return;
////            scriptPlayer.IncreaseCheckPoint();
////            GridGenerator.Instance.CheckIfPlayerAboveOfMiddleLevel(transform.position, scriptPlayer.checkPoint);
////        }
////    }
////}
//
//
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int checkPointNum;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;

        var status = PlayerManager.Instance.Status;
        if (status.CheckPointCount >= checkPointNum) return;

        status.IncreaseCheckpoint();
        //UIManager.Instance.UpdateGameplayScore(status.CheckPoint);
    }
}