using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.STP;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    private int lastFailIndex;
    private int currentCheckPoint;
    
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
    }
    #endregion
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
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
    public void OnPlayerDied(int score)
    {
        var status = PlayerManager.Instance.Status;

        bool beforeProgress = status.CheckPoint < gameConfig.progressSaveCheckpoint;
        bool tooManyDeaths = status.DeathCount >= gameConfig.maxDeathsBeforeFullRestart;

        if (beforeProgress && tooManyDeaths)
        {
            lastFailIndex = 0;
            RestartGame();
            return;
        }

        if (status.CheckPoint >= gameConfig.progressSaveCheckpoint)
        {
            lastFailIndex = LevelManager.Instance.GetLastFailIndex();
            //UIManager.Instance.ShowLosingPanel(score);
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
}
