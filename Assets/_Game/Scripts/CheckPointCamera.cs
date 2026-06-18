using UnityEngine;

public class CheckPointCamera : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<PlayerController>().OnCameraCheckPointChange?.Invoke(transform.position);
        }
    }
}
