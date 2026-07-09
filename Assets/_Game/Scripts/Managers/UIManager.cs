using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIAnimations UIAnimations;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject PlayingPanel;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject FirePanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject LosingPanel;

    [SerializeField] private Button b_pause, b_play, b_shop;
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
        //GameEvents.OnGameStart += Init;
        Init();
        PlayerManager.Instance.Input.OnFirstTouch -= OnClick;
        PlayerManager.Instance.Input.OnFirstTouch += OnClick;
        GameEvents.OnGameEnd += OnGameEnd;
        //GameEvents.OnPlayButtonClick += OnClickPlayButtonAtPause;
        //GameEvents.OnPauseButtonClick += OnClickPauseButton;
    }

    private void OnDisable()
    {
        //GameEvents.OnGameStart -= Init;

        GameEvents.OnGameEnd -= OnGameEnd;
        //GameEvents.OnPlayButtonClick -= OnClickPlayButtonAtPause;
        //GameEvents.OnPauseButtonClick -= OnClickPauseButton;
    }

    private void Init()
    {
        HideAll();
        MainPanel.SetActive(true);
        UIAnimations.OnGameStart();
        ResetListeners();
        b_pause.onClick.AddListener(OnClickPauseButton);
        b_play.onClick.AddListener(OnClickPlayButtonAtPause);
    }
    private void ResetListeners()
    {
        b_pause.onClick.RemoveAllListeners();
        b_play.onClick.RemoveAllListeners();
    }
    private void HideAll()
    {
        MainPanel.SetActive(false);
        PlayingPanel.SetActive(false);
        PausePanel.SetActive(false);
        ShopPanel.SetActive(false);
        FirePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        LosingPanel.SetActive(false);
    }
    public void OnClick()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(AAA());
    }
    public void OnClickPlayButtonAtShop()
    {
        ShopPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
    public void OnClickPlayButtonAtPause()
    {
        PausePanel.SetActive(false);
        PlayingPanel.SetActive(true);
    }

    public void OnClickCloseButton()
    {
        HideAll();
        MainPanel.SetActive(true);
    }

    public void OnClickFireButton()
    {
        HideAll();
        FirePanel.SetActive(true);
    }
    public void OnClickShopButton()
    {
        HideAll();
        ShopPanel.SetActive(true);
    }
    public void OnClickPauseButton()
    {
        HideAll();
        PausePanel.SetActive(true);
    }

    public void OnClickSettingButton()
    {
        HideAll();
        SettingsPanel.SetActive(true);
    }

    public void OnGameEnd()
    {
        HideAll();
        LosingPanel.SetActive(true);
    }
    private IEnumerator AAA()
    {
        UIAnimations.OnPlayerClick();
        yield return new WaitForSeconds(0.5f);
        MainPanel.SetActive(false);
        PlayingPanel.SetActive(true);
    }
}
