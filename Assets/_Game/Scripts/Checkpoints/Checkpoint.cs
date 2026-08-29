
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkPointNum;
    Level parentLevel; 
    bool inactive = false;
    private void Start()
    {
        CheckpointManager.Instance.OnReplay -= OnReplay;
        CheckpointManager.Instance.OnReplay += OnReplay;

        parentLevel = GetComponentInParent<Level>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(inactive) return;
        if (!collider.CompareTag("Player")) return;

        LevelManager.Instance.IncreaseCheckpoint();
        UIManager.Instance.SetScore(LevelManager.Instance.TotalCheckpoints.ToString());
        parentLevel.IsPlayerInTheLevel = true;
        inactive = true;
        //var status = PlayerManager.Instance.Status;
        //if (status.CheckPointCount >= checkPointNum) return;
        //status.IncreaseCheckpoint();
    }
    private void OnReplay()
    {
        inactive = false;
    }
}