using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.STP;

public class GameManager : SingletonGame<GameManager>
{
    [SerializeField] private GameConfig gameConfig;
    private int lastFailIndex = -1;
    private int deathsAtCurrentStage;
    private Coroutine respawnCoroutine;

    public event Action OnPlayerDeath;
    public event Action<Transform> OnReplay;
    private CurrentState currentState = CurrentState.Playing;
    public CurrentState CurrentState => currentState;

    #region Singleton
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEvents.OnPause -= OnPause;
        GameEvents.OnPause += OnPause;
        GameEvents.OnContinue -= OnContinue;
        GameEvents.OnContinue += OnContinue;
        GameEvents.OnRestart -= RestartRun;
        GameEvents.OnRestart += RestartRun;
        PlayerManager.Instance.OnDeath -= OnDeath;
        PlayerManager.Instance.OnDeath += OnDeath;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion
    public void RaiseOnReplayCamera(Transform t) => OnReplay?.Invoke(t);
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

        if (totalCheckpoints >= gameConfig.progressSaveCheckpoint)
        {
            //LevelManager.Instance.ComputeCurrentLevelIndex();
            GameEvents.RaiseOnGameLose();
            return;
        }
        deathsAtCurrentStage++;
        if (deathsAtCurrentStage >= gameConfig.maxDeathsBeforeFullRestart)
        {
            GameEvents.RaiseOnGameLose();
            return;
        }
        ReplayGame();

    }

    public void ReplayGame()
    {
        UIManager.Instance.SetHighestScore(LevelManager.Instance.TotalCheckpoints);
        CheckpointManager.Instance.RaiseOnReplay();
        PlayerManager.Instance.Spawn();
        LevelManager.Instance.TotalCheckpoints = 0;
        UIManager.Instance.SetScore("0");
        RaiseOnReplayCamera(PlayerManager.Instance.PlayerTransform);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //if (respawnCoroutine != null)
        //{
        //    StopCoroutine(respawnCoroutine);
        //    respawnCoroutine = null;
        //}
        //respawnCoroutine = StartCoroutine(RespawnDelay());
    }
    public void RestartRun()
    {
        deathsAtCurrentStage = 0;

        LevelManager.Instance.OnLevelsReady -= OnLevelsReady;
        LevelManager.Instance.OnLevelsReady += OnLevelsReady;
        LevelManager.Instance.Initialize(lastFailIndex);
        OnReplay?.Invoke(PlayerManager.Instance.PlayerTransform);
        lastFailIndex = -1;
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
        Time.timeScale = 1f;
        currentState = CurrentState.Playing;
    }
    //IEnumerator RespawnDelay()
    //{
    //    yield return new WaitForSeconds(0.8f);
    //    OnReplay?.Invoke(PlayerManager.Instance.PlayerTransform);
    //}
}
public enum CurrentState
{
    Playing,
    Paused,
    End,
}





//#region Singleton
//private static GameManager _instance;
//public static GameManager Instance
//{
//    get
//    {
//        if (_instance == null)
//        {
//            _instance = FindFirstObjectByType<GameManager>();
//            if (_instance == null)
//            {
//                Debug.LogWarning($"GameManager is not found in the scene!");
//            }
//        }
//        return _instance;
//    }
//}

//private void Awake()
//{
//    if (_instance != null && _instance != this)
//    {
//        Destroy(gameObject);
//        return;
//    }

//    _instance = this;

//    SceneManager.sceneLoaded += OnSceneLoaded;
//    GameEvents.OnPause -= OnPause;
//    GameEvents.OnPause += OnPause;
//    GameEvents.OnContinue -= OnContinue;
//    GameEvents.OnContinue += OnContinue;
//    GameEvents.OnRestart -= RestartRun;
//    GameEvents.OnRestart += RestartRun;
//    PlayerManager.Instance.OnDeath -= OnDeath;
//    PlayerManager.Instance.OnDeath += OnDeath;
//}
//#endregion