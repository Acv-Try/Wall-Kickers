using UnityEngine;

public class CameraFollowing1 : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private float Xoffset;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Vector3 center;
    Vector3 NewPosition;

    private void Start()
    {
        transform.position = Target.position + cameraOffset;
    }

    void Update()
    {
        if (Target.position.x >= center.x + Xoffset || Target.position.x <= center.x - Xoffset)
        {
            NewPosition = new Vector3(Target.position.x, Target.position.y > transform.position.y ? Target.position.y : transform.position.y, -10); // Update the camera's position to follow the player
        }
        else
        {
            NewPosition = new Vector3(center.x, (Target.position.y > transform.position.y ? Target.position.y : transform.position.y ), -10); // Update the camera's position to follow the player
        }
        transform.position = Vector3.Lerp(transform.position, NewPosition, Time.deltaTime * 5f);
    }
}