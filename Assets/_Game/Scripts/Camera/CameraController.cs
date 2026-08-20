using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using TreeEditor;

public enum CameraState
{
    Frozen,
    Following,
    Returning
}
public class CameraController : MonoBehaviour
{
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float followingSpeed;
    [SerializeField] private float returningSpeed;
    [SerializeField] private GameObject deadLine;
    public Camera cameraMain;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;
    [SerializeField] private float shakeRandomness;

    private Vector3 center = Vector3.zero;
    private Transform playerTransform;
    private CameraState state = CameraState.Following;

    public void SetInitial(Vector3 startCenter, Transform _playerTransform)
    {
        center = startCenter;
        Debug.Log($"{center.x};{center.y};{center.z}");
        playerTransform = _playerTransform;
        transform.position = new Vector3(center.x, (center.y*0) + yOffset, -10f); ;
        Debug.Log($"{transform.position}");
        //SetDeadlinePosition();
    }
    public void SetCenter(Vector3 newCenter) => center = newCenter;
    public void Freeze() => state = CameraState.Frozen;
    public void HideDeadline() => deadLine.SetActive(false);
    public void ShowDeadline() => deadLine.SetActive(true);
    public void Shake() => transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);
    public CameraState State => state;
    public IEnumerator ReturnToCenter()
    {
        state = CameraState.Returning;
        var target = new Vector3(center.x, (center.y*0) + yOffset, -10f);
        while (state == CameraState.Returning && Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target, returningSpeed * Time.deltaTime);
            yield return null;
        }
        if (state == CameraState.Returning)
        {
            transform.position = target;
            state = CameraState.Following;
        }

    }
    private void Update()
    {
        if (state != CameraState.Following || playerTransform == null) return;

        float y = Mathf.Max(transform.position.y, playerTransform.position.y + yOffset);
        float x = center.x;

        if (playerTransform.position.x > center.x + xOffset)
            x = playerTransform.position.x - xOffset / 3;
        else if (playerTransform.position.x < center.x - xOffset)
            x = playerTransform.position.x + xOffset / 3;

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(x, y, -10f),
            followingSpeed * Time.deltaTime
        );
    }
}