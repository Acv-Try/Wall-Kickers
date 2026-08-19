
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int checkPointNum;
    Level parentLevel; 
    bool wasActivate = false;
    private void Start()
    {
        parentLevel = GetComponentInParent<Level>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(wasActivate) return;
        if (!collider.CompareTag("Player")) return;

        LevelManager.Instance.IncreaseCheckpoint();
        UIManager.Instance.SetScore(LevelManager.Instance.TotalCheckpoints.ToString());
        parentLevel.IsPlayerInTheLevel = true;
        wasActivate = true;
        //var status = PlayerManager.Instance.Status;
        //if (status.CheckPointCount >= checkPointNum) return;
        //status.IncreaseCheckpoint();
    }
}