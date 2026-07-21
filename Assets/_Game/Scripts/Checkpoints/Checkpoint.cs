
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