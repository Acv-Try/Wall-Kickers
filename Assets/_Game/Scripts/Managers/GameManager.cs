using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;

    int progress, maxDeaths;
    #region
    private static GameManager instance;
    public static GameManager Instance => instance;

    private int currentCheckPoint;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion
    private void OnDestroy()
    {
        LevelManager.Instance.OnFirstLevelReady -= OnFirstLevelReady;
        PlayerManager.Instance.OnDied -= OnPlayerDied;
    }

    private void Start()
    {
        progress = gameConfig.progressSaveCheckpoint;
        maxDeaths = gameConfig.maxDeathsBeforeFullRestart;

        LevelManager.Instance.OnFirstLevelReady += OnFirstLevelReady;
        PlayerManager.Instance.OnDied += OnPlayerDied;
    }

    private void OnFirstLevelReady(Vector3 center, Vector3 spawnPosition)
    {
        PlayerManager.Instance.Initialize(spawnPosition, progress, maxDeaths);
        CameraController.Instance.Initialize(center, PlayerManager.Instance.Status.transform);
    }

    private void OnPlayerDied(int score)
    {
        var status = PlayerManager.Instance.Status;

        bool beforeProgress = status.CheckPoint < progress;
        bool tooManyDeaths = status.DeathCount >= maxDeaths;

        if (beforeProgress && tooManyDeaths)
        {
            RestartGame();
            return;
        }

        if (status.CheckPoint >= progress)
        {
            //UIManager.Instance.ShowLosingPanel(score);
            return;
        }
    }

    public void RestartGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        //SceneManager.LoadScene(0);
    }
}
