using System;
using System.Collections;
using UnityEngine;
using UnityEngine.iOS;
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
        UIEvents.OnPause -= OnPause;
        UIEvents.OnPause += OnPause;
        UIEvents.OnContinue -= OnContinue;
        UIEvents.OnContinue += OnContinue;
        UIEvents.OnRestart -= RestartRun;
        UIEvents.OnRestart += RestartRun;
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
        LevelManager.Instance.FirstLevelCameraCenter,
        LevelManager.Instance.FirstLevelLeftOffset,
        LevelManager.Instance.FirstLevelRightOffset,
        PlayerManager.Instance.PlayerTransform
        );
    }
    //absolutely need to be rewritten 
    public void OnDeath()
    {
        OnPlayerDeath?.Invoke();
        int totalCheckpoints = LevelManager.Instance.TotalCheckpoints;
        Debug.Log("total --"+totalCheckpoints);
        if (totalCheckpoints >= gameConfig.progressSaveCheckpoint)
        {
            //LevelManager.Instance.ComputeCurrentLevelIndex();
            Debug.Log($"total {totalCheckpoints}, raise on game lose");
            UIEvents.RaiseOnGameLose();
            return;
        }
        deathsAtCurrentStage++;
        if (deathsAtCurrentStage >= gameConfig.maxDeathsBeforeFullRestart)
        {
            UIEvents.RaiseOnGameLose();
            return;
        }
        ReplayGame();

    }

    public void ReplayGame()
    {
       if(respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }
        respawnCoroutine = StartCoroutine(RespawnDelay());
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
    private IEnumerator RespawnDelay()
    {
        UIManager.Instance.SetHighestScore(LevelManager.Instance.TotalCheckpoints);
        UIManager.Instance.SetScore("0");
        CheckpointManager.Instance.RaiseOnReplay();
        LevelManager.Instance.TotalCheckpoints = 0;
        yield return new WaitForSeconds(0.8f);
        PlayerManager.Instance.Spawn();
        RaiseOnReplayCamera(PlayerManager.Instance.PlayerTransform);
        UIEvents.RaiseOnReplay();
    }
}
public enum CurrentState
{
    Playing,
    Paused,
    End,
}