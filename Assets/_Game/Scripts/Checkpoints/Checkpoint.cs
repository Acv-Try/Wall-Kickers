
using UnityEditor.PackageManager;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
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
        if (inactive) return;
        if (!collider.CompareTag("Player")) return;

        CheckpointManager.Instance.ScoreCompute();

        //>>>
        parentLevel.IncreaseCount();
        //>>>

        inactive = true;
    }
    private void OnReplay() => inactive = false;

}