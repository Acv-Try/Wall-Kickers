using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIAnimations UIAnimations;


    [SerializeField]
    private Button
       B_Pause,
        B_PlayPauseMenu,
        B_PlayLoseMenu,
        B_Settings_PauseMenu,
        B_Settings_LoseMenu,
        B_CloseSettings_PauseMenu,
        B_CloseSettings_LoseMenu;
    //B_GameAudio_PauseMenu,
    //B_GameAudio_SettingsMenu;
    [SerializeField] private TextMeshProUGUI TMP_Score, TMP_HighestScore;

    private int highestScore = 0;
    //private bool isGameStarted = false;
    private Coroutine coroutine;
    #region Singleton
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UIManager>();
                if (_instance == null)
                {
                    Debug.LogWarning($"UIManager is not found in the scene!");
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

    public void Initialize()
    {
        Init();
        //PlayerManager.Instance.Input.OnFirstTouch -= OnStart;
        //PlayerManager.Instance.Input.OnFirstTouch += OnStart;
        PlayerEvents.OnFirstTouch -= OnStart;
        PlayerEvents.OnFirstTouch += OnStart;
        GameEvents.OnGameLose -= OnGameLose;
        GameEvents.OnGameLose += OnGameLose;
    }


    private void Init()
    {
        highestScore = PlayerPrefs.GetInt("HighestScore", 1);
        TMP_HighestScore.text = highestScore.ToString();
        UIAnimations.OnGameLaunch();
        ResetListeners();
        B_Pause.onClick.AddListener(GameEvents.RaiseOnPause);
        B_Pause.onClick.AddListener(UIAnimations.OnPause);

        B_PlayPauseMenu.onClick.AddListener(GameEvents.RaiseOnContinue);
        B_PlayPauseMenu.onClick.AddListener(UIAnimations.OnContinue);

        B_PlayLoseMenu.onClick.AddListener(GameEvents.RaiseOnRestart);
        B_PlayLoseMenu.onClick.AddListener(UIAnimations.OnRestart);

        B_CloseSettings_PauseMenu.onClick.AddListener(UIAnimations.OpenSettingsFromPauseMenu);

        B_CloseSettings_LoseMenu.onClick.AddListener(UIAnimations.OpenSettingsFromLoseMenu);

    }
    private void ResetListeners()
    {
        B_Pause.onClick.RemoveAllListeners();
        B_PlayPauseMenu.onClick.RemoveAllListeners();
    }
    private void OnStart()
    {
        SetScore("0");
        SetHighestScore(highestScore);
        UIAnimations.OnStart();
        GameEvents.RaiseOnGameLaunch();
    }
    public void OnGameLose()
    {
        B_Pause.interactable = false;
        UIAnimations.OnLose();
    }

    public void SetScore(string value)
    {
        TMP_Score.text = value;
    }
    public void SetHighestScore(int score)
    {
        if (score > highestScore)
        {
            PlayerPrefs.SetInt("HighestScore", score);
            string value = score.ToString();
            TMP_HighestScore.text = value;
        }

    }

    //ad a logic to connect the restart button click to the restart logic.
}
