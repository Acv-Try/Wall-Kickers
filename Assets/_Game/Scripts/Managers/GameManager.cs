//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static UnityEngine.Rendering.STP;
//
//public class GameManager : MonoBehaviour
//{
//    [SerializeField] private GameConfig gameConfig;
//
//    private int lastFailIndex;
//    #region
//    private static GameManager instance;
//    public static GameManager Instance => instance;
//
//    private int currentCheckPoint;
//
//    private void Awake()
//    {
//        if (instance != null && instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//
//        instance = this;
//
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }
//    #endregion
//    private void OnDestroy()
//    {
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }
//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        LevelManager.Instance.OnLevelsReady += OnLevelsReady;
//        LevelManager.Instance.Initialize(lastFailIndex);
//    }
//    private void OnLevelsReady()
//    {
//        LevelManager.Instance.OnLevelsReady -= OnLevelsReady;
//
//        PlayerManager.Instance.Initialize(
//            LevelManager.Instance.FirstLevelSpawnPos,
//        gameConfig.progressSaveCheckpoint,
//            gameConfig.maxDeathsBeforeFullRestart
//        );
//
//        CameraController.Instance.Initialize(
//            LevelManager.Instance.FirstLevelCenter,
//            PlayerManager.Instance.Status.transform
//        );
//    }
//    public void OnPlayerDied(int score)
//    {
//        var status = PlayerManager.Instance.Status;
//
//        bool beforeProgress = status.CheckPoint < gameConfig.progressSaveCheckpoint;
//        bool tooManyDeaths = status.DeathCount >= gameConfig.maxDeathsBeforeFullRestart;
//
//        if (beforeProgress && tooManyDeaths)
//        {
//            lastFailIndex = 0;
//            RestartGame();
//            return;
//        }
//
//        if (status.CheckPoint >= gameConfig.progressSaveCheckpoint)
//        {
//            lastFailIndex = LevelManager.Instance.GetLastFailIndex();
//            //UIManager.Instance.ShowLosingPanel(score);
//            return;
//        }
//    }
//
//    public void RestartGame()
//    {
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }
//
//    public void GoToMenu()
//    {
//        SceneManager.LoadScene(0);
//    }
//}
//