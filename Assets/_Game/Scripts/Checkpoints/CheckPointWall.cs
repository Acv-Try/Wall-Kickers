using TMPro;
using UnityEngine;

public class CheckpointWall : MonoBehaviour
{
    [SerializeField] private Level parentLevel;
    [SerializeField] private TextMeshProUGUI checkpointText;
    [SerializeField] private Animator animator;

    private static string animationName = "PlayerIsOn";
    bool inactive = false;
    private void Start()
    {
        CheckpointManager.Instance.OnReplay -= OnReplay;
        CheckpointManager.Instance.OnReplay += OnReplay;

        //>>>
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (inactive) return;
        if (!collision.collider.CompareTag("Player")) return;

        Complete();
    }
    public void SetCheckPointText(int value) => checkpointText.text = value.ToString();

    private void OnReplay() => inactive = false;

    //>>>
    public void CompleteViaSkip()
    {
        if (inactive) return;
        Complete();
    }
    private void Complete()
    {
        parentLevel.IncreaseCount();
        CheckpointManager.Instance.ScoreCompute();
        animator.SetTrigger(animationName);
        inactive = true;
    }
}