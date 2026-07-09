using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject PlayingPanel;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject FirePanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject LosingPanel;

    private bool isGameStarted = false;

    public void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        GameEvents.OnGameStart += Init;
        GameEvents.OnGameEnd += OnGameEnd;
        GameEvents.OnPlayButtonClick += OnClickPlayButtonAtPouse;
        GameEvents.OnPauseButtonClick += OnClickPauseButton;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStart -= Init;
        GameEvents.OnGameEnd -= OnGameEnd;
        GameEvents.OnPauseButtonClick -= OnClickPauseButton;
    }

    private void Init()
    {
        HideAll();
        MainPanel.SetActive(true);
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

    public void OnClickPlayButtonAtShop()
    {
        ShopPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void OnClickPlayButtonAtPouse()
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
}
