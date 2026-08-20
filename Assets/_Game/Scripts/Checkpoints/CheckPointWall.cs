using TMPro;
using UnityEngine;

public class CheckPointWall : MonoBehaviour
{
    [SerializeField] private TextMeshPro checkPointText;

    private CheckPoint activeCheckPoint;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;

        var status = PlayerManager.Instance.Status;
        if (int.Parse(checkPointText.text) <= status.CheckPoint) return;

        activeCheckPoint = this.GetComponent<CheckPoint>();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;

        var rb = collider.GetComponent<Rigidbody2D>();
        if (rb.linearVelocityY < 0)
            activeCheckPoint = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        if (activeCheckPoint == null) return;

        PlayerManager.Instance.IncreaseCheckpoint();
        //UIManager.Instance.UpdateGameplayScore(status.CheckPoint);
        activeCheckPoint = null;
    }

    public void SetCheckPointText(int checkpoint)
    {
        string text = checkpoint.ToString();
        checkPointText.text = text;
    }
}