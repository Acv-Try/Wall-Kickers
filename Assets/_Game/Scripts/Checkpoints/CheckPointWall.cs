using TMPro;
using UnityEngine;

public class CheckpointWall : MonoBehaviour
{
    [SerializeField] private Level parentLevel;
    [SerializeField] private TextMeshProUGUI checkpointText;
    [SerializeField] private Animator animator;

    private Checkpoint activeCheckPoint;
    private static string animationName = "PlayerIsOn";
    bool inactive = false;
    private void Start()
    {
        CheckpointManager.Instance.OnReplay -= OnReplay;
        CheckpointManager.Instance.OnReplay += OnReplay;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (inactive) return;
        if (!collision.collider.CompareTag("Player")) return;

        LevelManager.Instance.IncreaseCheckpoint();
        parentLevel.IsPlayerInTheLevel = false;
        animator.SetTrigger(animationName);
        UIManager.Instance.SetScore(LevelManager.Instance.TotalCheckpoints.ToString());
        inactive = true;
    }
    public void SetCheckPointText(int value)
    {
        checkpointText.text = value.ToString();
    }
    private void OnReplay()
    {
        inactive = false;
    }
}