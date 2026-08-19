using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.STP;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    private int lastFailIndex;
    private int currentCheckPoint;
    private Coroutine respawnCoroutine;

    public event Action OnPlayerDeath; 
    public event Action<Transform> OnReplay; 
    private CurrentState currentState = CurrentState.Playing;
    public CurrentState CurrentState => currentState;
    
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogWarning($"GameManager is not found in the scene!");
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

        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEvents.OnPause -= OnPause;
        GameEvents.OnPause += OnPause;
        GameEvents.OnContinue -= OnContinue;
        GameEvents.OnContinue += OnContinue;
        PlayerManager.Instance.OnDeath -= OnDeath;
        PlayerManager.Instance.OnDeath += OnDeath;
    }
    #endregion

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LevelManager.Instance.OnLevelsReady -= OnLevelsReady;
        LevelManager.Instance.OnLevelsReady += OnLevelsReady;
        LevelManager.Instance.Initialize();
        UIManager.Instance.Initialize();
    }
    private void OnLevelsReady()
    {
        LevelManager.Instance.OnLevelsReady -= OnLevelsReady;

        PlayerManager.Instance.Initialize(LevelManager.Instance.FirstLevelSpawnPos);

        CameraManager.Instance.Initialize(
            LevelManager.Instance.FirstLevelCenter,
            PlayerManager.Instance.PlayerTransform
        );
    }
    //absolutely need to be rewritten 
    public void OnDeath()
    {
        OnPlayerDeath?.Invoke();
        int totalCheckpoints = LevelManager.Instance.TotalCheckpoints;

        //int deaths = PlayerManager.Instance.Status.DeathCount;
        int deaths = 0;
        if (totalCheckpoints >= gameConfig.progressSaveCheckpoint)
        {
            LevelManager.Instance.ComputeCurrentLevelIndex();
            GameEvents.RaiseOnGameLose();
            return;
        }
        if (deaths < gameConfig.progressSaveCheckpoint)
        {
            ReplayGame();
            return;
        }
        else
        {
            GameEvents.RaiseOnGameLose();
            return;
        }

    }

    public void ReplayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerManager.Instance.Spawn();
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }
        respawnCoroutine = StartCoroutine(RespawnDelay());
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
    private void OnPause()
    {
        currentState = CurrentState.Paused;
        Time.timeScale = 0f;
    }
    private void OnContinue()
    {
        Debug.Log("enter onContinue, game manager");

        Time.timeScale = 1f;
        currentState = CurrentState.Playing;
    }
    //
    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(0.8f);
        OnReplay?.Invoke(PlayerManager.Instance.PlayerTransform);
    }
}
public enum CurrentState
{
    Playing,
    Paused,
    End,
}
