using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private IPlayerStatus status;
    private IPlayerController controller;
    private IPlayerInput input;
    public event Action OnPlayerDied;
    public event Action<int, int> OnCheckpointReached;
    public event Action<Vector3> OnCameraCheckpointReached;

    private SoundData characterSoundData;
    private Vector3 _spawnPosition;
    private int _progress, _maxDeaths;
    public IPlayerStatus Status { get; private set; }
    public IPlayerInput Input { get; private set; }
    public Transform PlayerTransform { get; private set; }

    #region Singleton
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerManager>();
                if (_instance == null)
                {
                    Debug.LogWarning($"PlayerManager is not found in the scene!");
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

    private void SubOn()
    {
        status.OnDeath += HandleDied;
        status.OnCheckpointReached += HandleCheckpointReached;
        status.OnCameraCheckpointReached += HandleCameraCheckpointReached;
    }

    private void OnDestroy()
    {
        if (status == null) return;
        status.OnDeath -= HandleDied;
        status.OnCheckpointReached -= HandleCheckpointReached;
        status.OnCameraCheckpointReached -= HandleCameraCheckpointReached;
    }

    public void Initialize(Vector3 spawnPosition, int progress, int maxDeaths)
    {
        _spawnPosition = spawnPosition;
        _progress = progress;
        _maxDeaths = maxDeaths;
        var playerGO = Instantiate(Player, spawnPosition, Quaternion.identity);
        controller = playerGO.GetComponent<IPlayerController>();
        status = playerGO.GetComponent<IPlayerStatus>();
        input = playerGO.GetComponent<IPlayerInput>();

        Input = input;
        Status = status;
        status.Initialize(_spawnPosition);
        //controller.Initialize();
        PlayerTransform = playerGO.transform;
        SubOn();

        characterSoundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Character);

    }

    public void TriggerCameraCheckpoint(Vector3 position)
    {
        status.TriggerCameraCheckpoint(position);
    }
    public void IncreaseCheckpoint()
    {
        status.IncreaseCheckpoint();
    }
    private void HandleDied()
    {
        OnPlayerDied?.Invoke();
        bool tooManyDeaths = status.DeathCount >= _maxDeaths;
        bool beforeProgress = status.CheckPoint < _progress;

        if (beforeProgress && tooManyDeaths)
        {
            GameManager.Instance.OnPlayerDied(status.CheckPoint);
            return;
        }

        if (status.CheckPoint >= _progress)
        {
            GameManager.Instance.OnPlayerDied(status.CheckPoint);
            return;
        }
        StartCoroutine(RespawnSequence());
    }

    private void HandleCheckpointReached(int total, int countInLevel)
    {
        OnCheckpointReached?.Invoke(total, countInLevel);
    }

    private void HandleCameraCheckpointReached(Vector3 position)
    {
        OnCameraCheckpointReached?.Invoke(position);
    }

    private System.Collections.IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(0.8f);
        status.Initialize(_spawnPosition);
    }
}