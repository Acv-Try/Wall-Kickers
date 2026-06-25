using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerAnimator playerAnimator;

    public event Action<int> OnDied;
    public event Action OnRespawned;
    public event Action<int, int> OnCheckpointReached;
    public event Action<Vector3> OnCameraCheckpointReached;
    public event Action OnCameraShake;

    private SoundData characterSoundData;

    private int _progress, _maxDeaths;
    public PlayerStatus Status => status;
    #region
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerManager>();
                if (_instance != null)
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

        status.OnDied += HandleDied;
        status.OnRespawned += HandleRespawned;
        status.OnCheckpointReached += HandleCheckpointReached;
        status.OnCameraCheckpointReached += HandleCameraCheckpointReached;
    }
    #endregion



    private void OnDestroy()
    {
        status.OnDied -= HandleDied;
        status.OnRespawned -= HandleRespawned;
        status.OnCheckpointReached -= HandleCheckpointReached;
        status.OnCameraCheckpointReached -= HandleCameraCheckpointReached;
    }

    public void Initialize(Vector3 spawnPosition, int progress, int maxDeaths)
    {
        _progress = progress;
        _maxDeaths = maxDeaths;
        characterSoundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Character);
        status.Initialize(spawnPosition);
    }


    public void PlayJumpAudio()
    {
        AudioManager.Instance.Play(characterSoundData, EType_Gameplay_SFX.C_Monkey_Jump);
        AudioManager.Instance.PlayAndTrack(characterSoundData, EType_Gameplay_SFX.C_Monkey_JumpEffect);
    }

    public void PlayDeathAudio()
    {
        AudioManager.Instance.Stop(EType_Gameplay_SFX.C_Monkey_JumpEffect);
        AudioManager.Instance.Play(characterSoundData, EType_Gameplay_SFX.C_Monkey_Death);
        AudioManager.Instance.Play(characterSoundData, EType_Gameplay_SFX.C_Monkey_Death_Explosion);
    }

    public void PlayLandAudio()
    {
        AudioManager.Instance.Stop(EType_Gameplay_SFX.C_Monkey_JumpEffect);
    }


    private void HandleDied()
    {
        controller.Rb.simulated = false;
        playerAnimator.SetVisible(false);
        playerAnimator.PlayBurstEffect();
        PlayDeathAudio();
        OnCameraShake?.Invoke();
        OnDied?.Invoke(status.CheckPoint);

        bool tooManyDeaths = status.DeathCount >= _maxDeaths;
        bool beforeProgress = status.CheckPoint < _progress;

        if (beforeProgress && tooManyDeaths) return; // GameManager handles full restart
        if (status.CheckPoint >= _progress) return; // GameManager shows losing panel

        StartCoroutine(RespawnSequence());
    }

    private void HandleRespawned()
    {
        OnRespawned?.Invoke();
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
        yield return new WaitForSeconds(1f);
        status.ResetStats();
        status.Respawn();
    }
}