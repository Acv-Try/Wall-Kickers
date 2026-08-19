using TMPro;
using UnityEngine;

public class CheckPointWall : MonoBehaviour
{
    [SerializeField] private Level parentLevel;
    [SerializeField] private TextMeshProUGUI checkpointText;
    [SerializeField] private Animator animator;

    private CheckPoint activeCheckPoint;
    private static string animationName = "PlayerIsOn";
    bool wasActivate = false;
    //private void OnTriggerEnter2D(Collider2D collider)
    //{
    //    if (!collider.CompareTag("Player")) return;

    //    var status = PlayerManager.Instance.Status;
    //    if (int.Parse(checkPointText.text) <= status.CheckPoint) return;

    //    activeCheckPoint = GetComponent<CheckPoint>();

    //}

    //private void OnTriggerExit2D(Collider2D collider)
    //{
    //    if (!collider.CompareTag("Player")) return;

    //    var rb = collider.GetComponent<Rigidbody2D>();
    //    if (rb.linearVelocityY < 0)
    //        activeCheckPoint = null;
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (wasActivate) return;
        if (!collision.collider.CompareTag("Player")) return;

        LevelManager.Instance.IncreaseCheckpoint();
        parentLevel.IsPlayerInTheLevel = false;
        animator.SetTrigger(animationName);
        UIManager.Instance.SetScore(LevelManager.Instance.TotalCheckpoints.ToString());
        wasActivate = true;
    }

    public void SetCheckPointText(int value)
    {
        checkpointText.text = value.ToString();
    }
}