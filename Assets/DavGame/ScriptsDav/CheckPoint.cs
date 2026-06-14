using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<Player>().OnCameraCheckPointChange?.Invoke(transform.position);
        }
    }
}
