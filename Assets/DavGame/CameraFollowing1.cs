using UnityEngine;

public class CameraFollowing1 : MonoBehaviour
{
    [SerializeField] private Transform Target;

    [SerializeField] private float Xoffset;
    [SerializeField] private float YOffset;

    [SerializeField] private Vector3 center;
    [SerializeField] private float speed;
    
    Vector3 NewPosition;
    
    private void Start()
    {
        transform.position = new Vector3(center.x, center.y + YOffset, -10);
        Debug.DrawLine(
            new Vector3(center.x + Xoffset, center.y - 100f, center.z),
            new Vector3(center.x + Xoffset, center.y + 100f, center.z),
            Color.red,
            10f
        );

        Debug.DrawLine(
            new Vector3(center.x - Xoffset, center.y - 100f, center.z),
            new Vector3(center.x - Xoffset, center.y + 100f, center.z),
            Color.red,
            10f
        );
    }

    void Update()
    {
        float y = Mathf.Max(transform.position.y, Target.position.y + YOffset);

        float x = center.x;

        if (Target.position.x > center.x + Xoffset)
            x = Target.position.x - Xoffset/3;
        else if (Target.position.x < center.x - Xoffset)
            x = Target.position.x + Xoffset / 3;

        NewPosition = new Vector3(x, y, -10);

        transform.position = Vector3.Lerp(
            transform.position,
            NewPosition,
            speed * Time.deltaTime
        );
    }

    public void UpdateCenter(Vector3 newPos)
    {
        center = newPos;
    }
}