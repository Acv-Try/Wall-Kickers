using System.Collections.Generic;
using UnityEngine;

public class CheckPointCamera : MonoBehaviour
{
    [SerializeField] private float halfWidthXPos;
    [SerializeField] bool firstCheckpoint;
    private HalfWidth halfWidth;
    private bool inactive = false;

    private void Start()
    {
        halfWidth = GetComponentInChildren<HalfWidth>();
        halfWidth.SetXPos(halfWidthXPos);
        float rightOffset = halfWidth.transform.position.x - transform.position.x;
        float leftOffset = -rightOffset;

        CheckpointManager.Instance.OnReplay -= OnReplay;
        CheckpointManager.Instance.OnReplay += OnReplay;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (inactive) return;
        if (!collider.CompareTag("Player")) return;
        if (firstCheckpoint) CameraManager.Instance.SetState(CameraState.Following);
        float rightOffset = halfWidth.transform.position.x - transform.position.x;
        float leftOffset = -rightOffset;
        //float leftOffset = asymmetric
        //    ? leftBorderMarker.position.x - transform.position.x
        //    : -rightOffset;

        CameraBox.Instance.SetCenter(transform.position);
        CameraBox.Instance.SetOffsets(leftOffset, rightOffset);

        inactive = true;
    }
    public void OnReplay() => inactive = false;

}