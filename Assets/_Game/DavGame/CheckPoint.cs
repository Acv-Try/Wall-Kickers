using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPointNum;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            var scriptPlayer = collider.GetComponent<Player>();
            if (scriptPlayer.checkPoint >= checkPointNum) return;

            scriptPlayer.checkPoint = checkPointNum;
            GridGenerator.Instance.CheckIfPlayerAboveOfMiddleLevel(transform.position, checkPointNum);
        }
    }
}
