using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    void Update()
    {
        Vector3 pos = transform.position;
        pos.y -= Time.deltaTime * speed;
        transform.position = pos;
    }
}
