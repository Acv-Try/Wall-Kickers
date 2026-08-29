using UnityEngine;

public class CameraBox : SingletonGame<CameraBox>
{
    [SerializeField] private float yOffset;

    private Transform playerTransform;
    private float centerX;
    private float centerY;
    private float leftOffset;
    private float rightOffset;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public float CenterX => centerX;
    public float CenterY => centerY;

    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }
    public void SetCenter(Vector3 newCenter)
    {
        centerX = newCenter.x;
        centerY = newCenter.y;
    }
    public void SetOffsets(float newLeftOffset, float newRightOffset)
    {
        leftOffset = newLeftOffset;
        rightOffset = newRightOffset;
    }

    private void Update()
    {
        if (playerTransform == null) return;
        if (CameraManager.Instance.State != CameraState.Following) return;

        if (playerTransform.position.x > centerX + rightOffset)
            centerX = playerTransform.position.x - rightOffset;
        else if (playerTransform.position.x < centerX + leftOffset)
            centerX = playerTransform.position.x - leftOffset;

        centerY = Mathf.Max(centerY, playerTransform.position.y + yOffset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 left = new Vector3(centerX + leftOffset, centerY, 0f);
        Vector3 right = new Vector3(centerX + rightOffset, centerY, 0f);
        Gizmos.DrawLine(left + Vector3.down * 5f, left + Vector3.up * 5f);
        Gizmos.DrawLine(right + Vector3.down * 5f, right + Vector3.up * 5f);
        Gizmos.DrawLine(left + Vector3.up * 5f, right + Vector3.up * 5f); // top border only
    }
}
