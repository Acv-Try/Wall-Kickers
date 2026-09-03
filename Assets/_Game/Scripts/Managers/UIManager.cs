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
        B_CloseSettings_LoseMenu,
        B_Mute;
    //B_GameAudio_PauseMenu,
    //B_GameAudio_SettingsMenu;
    [SerializeField] private TextMeshProUGUI TMP_Score, TMP_HighestScore;

    private int highestScore = 0;
    //private bool isGameStarted = false;
    private Coroutine coroutine;
    private SoundData soundData;
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
        UIEvents.OnGameLose -= OnGameLose;
        UIEvents.OnGameLose += OnGameLose;
    }


    private void Init()
    {
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.UI);
        highestScore = PlayerPrefs.GetInt("HighestScore", 1);
        TMP_HighestScore.text = highestScore.ToString();
        UIAnimations.OnGameLaunch();
        ResetListeners();
        B_Pause.onClick.AddListener(CallOnPauseEvent);
        B_Pause.onClick.AddListener(UIAnimations.OnPause);
        B_Pause.onClick.AddListener(() => PlayUIClickSound(EType_UI_SFX.Icons));

        B_PlayPauseMenu.onClick.AddListener(CallOnContinueEvent);
        B_PlayPauseMenu.onClick.AddListener(UIAnimations.OnContinue);
        B_PlayPauseMenu.onClick.AddListener(() => PlayUIClickSound(EType_UI_SFX.Start_Button));
        
        B_PlayLoseMenu.onClick.AddListener(CallOnRestartEvent);
        B_PlayLoseMenu.onClick.AddListener(UIAnimations.OnRestart);
        B_PlayLoseMenu.onClick.AddListener(() => PlayUIClickSound(EType_UI_SFX.Start_Button));

        B_CloseSettings_PauseMenu.onClick.AddListener(UIAnimations.OpenSettingsFromPauseMenu);
        B_CloseSettings_PauseMenu.onClick.AddListener(() => PlayUIClickSound(EType_UI_SFX.Icons));

        B_CloseSettings_LoseMenu.onClick.AddListener(UIAnimations.OpenSettingsFromLoseMenu);

        B_Mute.onClick.AddListener(() => PlayUIClickSound(EType_UI_SFX.Audio_Button_Off));
        B_Mute.onClick.AddListener(CallMuteEvent);
        B_Mute.onClick.AddListener(UIAnimations.OnMute);
    }
    private void ResetListeners()
    {
        B_Pause.onClick.RemoveAllListeners();
        B_PlayPauseMenu.onClick.RemoveAllListeners();
    }
    private void OnStart()
    {
        SetScore("0");
        B_Pause.interactable = true;
        SetHighestScore(highestScore);
        UIAnimations.OnStart();
        UIEvents.RaiseOnGameLaunch();
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
    private void CallMuteEvent() => AudioManager.Instance.RaiseOnMute();
    private void CallOnContinueEvent() => UIEvents.RaiseOnContinue();
    private void CallOnPauseEvent() => UIEvents.RaiseOnPause();
    private void CallOnRestartEvent() => UIEvents.RaiseOnRestart();
    private void PlayUIClickSound(EType_UI_SFX type)
    {
        AudioManager.Instance.Play(soundData, type);
    }
} 