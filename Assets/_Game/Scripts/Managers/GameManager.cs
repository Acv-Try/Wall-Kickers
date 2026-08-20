using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.STP;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    private int lastFailIndex;
    private int currentCheckPoint;
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
        LevelManager.Instance.Initialize(lastFailIndex);
        UIManager.Instance.Initialize();
    }
    private void OnLevelsReady()
    {
        LevelManager.Instance.OnLevelsReady -= OnLevelsReady;

        PlayerManager.Instance.Initialize(
            LevelManager.Instance.FirstLevelSpawnPos,
        gameConfig.progressSaveCheckpoint,
            gameConfig.maxDeathsBeforeFullRestart
        );

        CameraManager.Instance.Initialize(
            LevelManager.Instance.FirstLevelCenter,
            //gameConfig.cameraCenter,
            PlayerManager.Instance.PlayerTransform
        );
    }
    //absolutely need to be rewritten 
    public void OnPlayerDied(int score)
    {
        var status = PlayerManager.Instance.Status;

        bool beforeProgress = status.CheckPoint < gameConfig.progressSaveCheckpoint;
        bool tooManyDeaths = status.DeathCount >= gameConfig.maxDeathsBeforeFullRestart;
        Debug.Log("Player died #" + tooManyDeaths);

        if (beforeProgress && tooManyDeaths)
        {
            Debug.Log("Player died 4 " + tooManyDeaths + " times. Need a lose panel");
            lastFailIndex = 0;
            RestartGame();
            return;
        }

        if (status.CheckPoint >= gameConfig.progressSaveCheckpoint)
        {
            lastFailIndex = LevelManager.Instance.GetLastFailIndex();
            //UIManager.Instance.ShowLosingPanel(score);
            GameEvents.RaiseOnGameLose();
            return;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}
public enum CurrentState
{
    Playing,
    Paused,
    End,
}
