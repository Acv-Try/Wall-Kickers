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
    [SerializeField] private float speed;
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
        playerTransform = _playerTransform;
        transform.position = new Vector3(center.x, center.y + yOffset, -10f); ;
        //SetDeadlinePosition();
    }
    public void SetCenter(Vector3 newCenter) => center = newCenter;
    public void Freeze() => state = CameraState.Frozen;
    public void HideDeadline() => deadLine.SetActive(false);
    public void ShowDeadline() => deadLine.SetActive(true);
    public void Shake() => transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);

    public IEnumerator ReturnToCenter()
    {
        state = CameraState.Returning;
        var target = new Vector3(center.x, center.y + yOffset, -10f);
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        state = CameraState.Following;

    }
    private void SetDeadlinePosition()
    {
        //Vector3 pos = deadLine.transform.position;
        //Vector3 offset = transform.position / 2;
        //deadLine.gameObject.transform.position = new Vector3(pos.x, offset.y - 2f, (pos.z * 0) + 10f);
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
            speed * Time.deltaTime
        );
    }

    //private bool isFrozen;
    //private bool isMovingToCenter;
    //private Coroutine cameraRoutine;
    //int initCount = 0;
    //private void OnDestroy()
    //{
    //    if (PlayerManager.Instance == null) return;
    //    PlayerManager.Instance.OnRespawn -= OnPlayerRespawned;
    //    PlayerManager.Instance.OnPlayerDied -= OnPlayerDied;
    //    PlayerManager.Instance.OnCameraCheckpointReached -= OnCheckpointReached;
    //    PlayerManager.Instance.OnCameraShake -= Shake;
    //}

    //public void Initialize(Vector3 startCenter, Transform player)
    //{
    //    Debug.Log($"CameraController.Initialize #{++initCount} on instance {GetInstanceID()}");
    //    isFrozen = false;
    //    playerTransform = player;

    //    PlayerManager.Instance.OnRespawn += OnPlayerRespawned;
    //    PlayerManager.Instance.OnPlayerDied += OnPlayerDied;
    //    PlayerManager.Instance.OnCameraCheckpointReached += OnCheckpointReached;
    //    PlayerManager.Instance.OnCameraShake += Shake;

    //    center = startCenter;
    //    transform.position = new Vector3(center.x, center.y + yOffset, -10f);
    //}

    //private void Update()
    //{
    //    if (isFrozen) return;
    //    if (playerTransform == null) return;

    //    if (isMovingToCenter) return;

    //    float y = Mathf.Max(transform.position.y, playerTransform.position.y + yOffset);
    //    float x = center.x;

    //    if (playerTransform.position.x > center.x + xOffset)
    //        x = playerTransform.position.x - xOffset / 3;
    //    else if (playerTransform.position.x < center.x - xOffset)
    //        x = playerTransform.position.x + xOffset / 3;

    //    transform.position = Vector3.Lerp(
    //        transform.position,
    //        new Vector3(x, y, -10f),
    //        speed * Time.deltaTime
    //    );
    //}
    //private void OnCheckpointReached(Vector3 newCenter)
    //{
    //    center = newCenter;
    //}

    //private void OnPlayerDied()
    //{
    //    Debug.Log("player died");
    //    isFrozen = true;
    //    isMovingToCenter = true;
    //    deadLine.SetActive(false);

    //}

    //public void OnPlayerRespawned()
    //{
    //    Debug.Log($"on player respawned - instance {GetInstanceID()}");
    //    //Debug.Log("on player respawned");
    //    isFrozen = false;
    //    if (cameraRoutine != null) StopCoroutine(cameraRoutine);
    //    cameraRoutine = StartCoroutine(MoveToInitPos());
    //}

    //public void Shake()
    //{
    //    transform.DOShakePosition(
    //        shakeDuration,
    //        shakeStrength,
    //        shakeVibrato,
    //        shakeRandomness
    //    );
    //}
    //IEnumerator MoveToInitPos()
    //{
    //    var target = new Vector3(center.x, center.y + yOffset, -10f);
    //    while (Vector3.Distance(transform.position, target) > 0.05f)
    //    {
    //        //Debug.Log("return to the player");
    //        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
    //        yield return null;
    //    }
    //    transform.position = target;
    //    isMovingToCenter = false;
    //    deadLine.SetActive(true);
    //    cameraRoutine = null;
    //}
}