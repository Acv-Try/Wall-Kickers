using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private IPlayerController controller;
    private IPlayerInput input;

    public event Action OnDeath;
    public event Action OnRespawn;
    public event Action<Vector3> OnPlayerPositionChange;
    
    private SoundData characterSoundData;
    private Vector3 _spawnPosition;
    private int _progress;
    private GameObject _player;

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

    public void RiseOnPlayerPositionChange(Vector3 playerPos) => OnPlayerPositionChange?.Invoke(playerPos);
    public void RiseOnDeath() => OnDeath?.Invoke();
    public void RiseOnRespawn() => OnRespawn?.Invoke();

    public void Initialize(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
        
        _player = Instantiate(Player, spawnPosition, Quaternion.identity);
        controller = _player.GetComponent<IPlayerController>();
        input = _player.GetComponent<IPlayerInput>();

        Input = input;
        PlayerTransform = _player.transform;
        
        characterSoundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Character);
    }
    public void OnDie()
    {
        Destroy(_player);
        RiseOnDeath();
    }
    public void Spawn()
    {
        Initialize(_spawnPosition);
        RiseOnRespawn();
    }
   
}