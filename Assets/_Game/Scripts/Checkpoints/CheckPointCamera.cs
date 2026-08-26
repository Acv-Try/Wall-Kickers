

using UnityEngine;

public class CheckPointCamera : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;
          collider.GetComponent<PlayerController>().Cameracontrolerincrease(transform.position);

          Destroy(gameObject);
    }
}