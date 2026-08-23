using UnityEngine;
using DG.Tweening;
using System.Collections;
//using Unity.VisualScripting.Dependencies.NCalc;
//using Unity.VisualScripting;

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
    [SerializeField] private GameObject deathLine;
    [SerializeField] private float deathlineOffset;
    public Camera cameraMain;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;
    [SerializeField] private float shakeRandomness;

    private Vector3 center = Vector3.zero;
    private Transform playerTransform;
    private CameraState state = CameraState.Following;
    private void Start()
    {
        CalcDeadlinePos();
    }
    public void SetInitial(Vector3 startCenter, Transform _playerTransform)
    {
        center = startCenter;
        playerTransform = _playerTransform;
        transform.position = new Vector3(center.x, (center.y*0) + yOffset, -10f);
        state = CameraState.Following;
    }
    public void SetTarget(Vector3 startCenter, Transform _playerTransform)
    {
        center = startCenter;
        playerTransform = _playerTransform;
    }
    private void CalcDeadlinePos()
    {
        float x = deathLine.transform.localPosition.x;
        float y = deathLine.transform.localPosition.y * 0 - deathlineOffset;
        float z = deathLine.transform.localPosition.z;
        deathLine.transform.localPosition = new Vector3(x,y,z);
    }
    public void Freeze() => state = CameraState.Frozen;
    public void HideDeadline() => deathLine.SetActive(false);
    public void ShowDeadline() => deathLine.SetActive(true);
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
        if (state != CameraState.Following || playerTransform == null)
        {
            return;
        }

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