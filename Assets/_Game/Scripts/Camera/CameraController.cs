using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float speed;
    [SerializeField] private GameObject deadLine;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;
    [SerializeField] private float shakeRandomness;

    private Vector3 center;
    private bool isFrozen;
    private bool isMovingToCenter;
    private Transform playerTransform;

    #region
    private static CameraController _instance;
    public static CameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CameraController>();
                if (_instance != null)
                {
                    Debug.LogWarning($"CameraController is not found in the scene!");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion
    private void OnDestroy()
    {
        PlayerManager.Instance.OnRespawned -= OnPlayerRespawned;
        PlayerManager.Instance.OnCameraCheckpointReached -= OnCheckpointReached;
        PlayerManager.Instance.OnCameraShake -= Shake;
    }

    public void Initialize(Vector3 startCenter, Transform player)
    {
        playerTransform = player;

        PlayerManager.Instance.OnRespawned += OnPlayerRespawned;
        PlayerManager.Instance.OnCameraCheckpointReached += OnCheckpointReached;
        PlayerManager.Instance.OnCameraShake += Shake;

        center = startCenter;
        transform.position = new Vector3(center.x, center.y + yOffset, -10f);
    }

    private void Update()
    {
        if (isFrozen) return;
        if (playerTransform == null) return;

        if (isMovingToCenter)
        {
            var target = new Vector3(center.x, center.y + yOffset, -10f);
            if (Vector3.Distance(transform.position, target) < 1f)
            {
                isMovingToCenter = false;
                deadLine.SetActive(true);
            }
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
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
            speed * Time.deltaTime
        );
    }

    private void OnCheckpointReached(Vector3 newCenter)
    {
        center = newCenter;
    }

    public void OnPlayerDied()
    {
        isFrozen = true;
        isMovingToCenter = true;
        deadLine.SetActive(false);
    }

    public void OnPlayerRespawned()
    {
        isFrozen = false;
    }

    public void Shake()
    {
        transform.DOShakePosition(
            shakeDuration,
            shakeStrength,
            shakeVibrato,
            shakeRandomness
        );
    }
}