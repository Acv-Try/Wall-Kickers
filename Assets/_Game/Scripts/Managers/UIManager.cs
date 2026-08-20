using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private bool isGameStarted = false;
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
        UIAnimations.OnGameLaunch();
        ResetListeners();
        B_Pause.onClick.AddListener(GameEvents.RaiseOnPause);
        B_Pause.onClick.AddListener(UIAnimations.OnPause);
        
        B_PlayPauseMenu.onClick.AddListener(GameEvents.RaiseOnContinue);
        B_PlayPauseMenu.onClick.AddListener(UIAnimations.OnContinue);
        
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
        UIAnimations.OnStart();
        GameEvents.RaiseOnGameLaunch();
    }
    public void OnGameLose()
    {
        UIAnimations.OnLose();
    }
    //ad a logic to connect the restart button click to the restart logic.
}
