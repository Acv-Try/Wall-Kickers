using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPointWall : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI checkPointText;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var playerScript = collision.collider.GetComponent<PlayerController>();
            playerScript.currentCheckPointWall = null;
            playerScript.IncreaseCheckPoint();
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            var playerScript = collider.GetComponent<PlayerController>();
            playerScript.currentCheckPointWall = null;
            if (Convert.ToInt16(checkPointText.text) <= playerScript.checkPoint) return;
            playerScript.currentCheckPointWall = this;
        } 
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && collider.GetComponent<Rigidbody2D>().linearVelocityY < 0)
        {
            var playerScript = collider.GetComponent<PlayerController>();
            playerScript.currentCheckPointWall = null;
        }
    }

    public void SetCheckPointText(int checkPoint)
    {
        checkPointText.text = checkPoint.ToString();
    }
}
