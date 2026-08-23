using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using Image = UnityEngine.UIElements.Image;
public class UIAnimations : MonoBehaviour
{
    [SerializeField] private Image tapImage;
    [SerializeField] private Sprite tapSprite1;
    [SerializeField] private Sprite tapSprite2;
    [SerializeField] private GameObject muteButton;
    [SerializeField] private Sprite muteSpriteActive;
    [SerializeField] private Sprite muteSpriteDisabled;
    [SerializeField] private float switchTime = 1f;
    [SerializeField]
    private UIPanelSlider
        P_Logo,
        P_Cup,
        P_ScoreCount,
        P_CollectedCoinInfo,
        P_PauseButton,
        P_MainMenu,
        P_PauseMenu,
        P_SettingsMenu,
        P_LoseMenu;
    [SerializeField] private GameObject startBackground, midGameBackground, P_Top, P_Bottom;
    [SerializeField] private float fadeDuration = 0.3f;

    private Image startBackgroundImage, midGameBackgroundImage;
    private bool isFirst = true;
    private bool isMute = true;
    Coroutine tapRoutine = null, onColorFadeRoutine = null, onStartRoutine = null, onLoseRoutine = null, onRestartRoutine = null;

    private void InitialConditions()
    {
        startBackground.SetActive(true);
        P_Logo.gameObject.SetActive(true);
        P_MainMenu.gameObject.SetActive(true);

        midGameBackground.SetActive(false);
        //P_Top.SetActive(true);
        //P_Bottom.SetActive(true);

        P_Cup.Hide();
        P_ScoreCount.Hide();
        P_PauseButton.Hide();
        //P_CollectedCoinInfo.Hide();

        P_Logo.Show();
        P_MainMenu.Show();
    }
    public void OnMute()
    {
        Sprite muteImage = muteButton.gameObject.GetComponent<Button>().spriteState.selectedSprite;
        muteImage = isFirst ? muteSpriteActive : muteSpriteDisabled;
        isMute = !isMute;
    }
    public void OnGameLaunch()
    {
        startBackgroundImage = startBackground.GetComponent<Image>();
        midGameBackgroundImage = midGameBackground.GetComponent<Image>();

        if (onStartRoutine == null)
        {
            onStartRoutine = StartCoroutine(OnStartRoutine(startBackgroundImage));
        }
        if (tapRoutine == null)
        {
            tapRoutine = StartCoroutine(ChangeSprite());
        }
    }
    public void OnStart()
    {
        if (tapRoutine != null)
        {
            StopCoroutine(tapRoutine);
            tapRoutine = null;
        }
        P_Logo.Hide();
        P_MainMenu.Hide();

        P_Cup.Show();
        P_ScoreCount.Show();
        P_PauseButton.Show();
    }
    public void OnLose()
    {
        P_PauseButton.Hide();
        startBackground.SetActive(true);
        if (onLoseRoutine == null)
        {
            onLoseRoutine = StartCoroutine(OnLoseRoutine());
        }
    }
    public void OnRestart()
    {
        startBackground.SetActive(true);
        P_Logo.Show();
        P_Cup.Hide();
        P_ScoreCount.Hide();
        if (onRestartRoutine == null)
        {
            onRestartRoutine = StartCoroutine(OnRestartRoutine());
        }
    }
    public void OnPause()
    {
        startBackground.SetActive(true);
        P_PauseMenu.gameObject.SetActive(true);
        if (onColorFadeRoutine == null)
        {
            onColorFadeRoutine = StartCoroutine(ColorFade(midGameBackgroundImage, 0.3f));
        }
        P_PauseButton.Hide();
        P_PauseMenu.Show();
    }
    public void OnContinue()
    {
        if (onColorFadeRoutine == null)
        {
            onColorFadeRoutine = StartCoroutine(ColorFade(midGameBackgroundImage, 0f));
        }
        startBackground.SetActive(false);
        P_PauseMenu.Hide();
        P_PauseButton.Show();
    }
    public void OpenSettingsFromPauseMenu()
    {
        P_PauseMenu.Hide();
        P_ScoreCount.Hide();
        P_Cup.Hide();
        P_SettingsMenu.Show();
    }
    public void OpenPauseMenuAndCloseSettingsMenu()
    {
        P_SettingsMenu.Hide();
        P_PauseMenu.Show();
        P_ScoreCount.Show();
        P_Cup.Show();
    }
    public void OpenSettingsFromLoseMenu()
    {
        P_PauseMenu.Hide();
        P_ScoreCount.Hide();
        P_Cup.Hide();
        //P_CollectedCoinInfo.Hide();
        P_SettingsMenu.Show();
    }
    public void OpenLoseMenuAndCloseSettingsMenu()
    {
        P_SettingsMenu.Hide();
        //P_CollectedCoinInfo.Show();
        P_ScoreCount.Show();
        P_Cup.Show();
        P_PauseMenu.Show();
    }
    IEnumerator ColorFade(Image background, float targetAlpha)
    {
        background.gameObject?.SetActive(true);
        Color startColor = background.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;
            background.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        background.color = targetColor;
        if (targetAlpha == 0f)
            background.gameObject.SetActive(false);
        onColorFadeRoutine = null;
    }

    IEnumerator OnRestartRoutine()
    {
        onRestartRoutine = null;
        //background gradient to orange and back to invisible
        yield return StartCoroutine(ColorFade(startBackgroundImage, 1f));
        StartCoroutine(ColorFade(midGameBackgroundImage, 0));
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(ColorFade(startBackgroundImage, 0f));
        //P_CollectedCoinInfo.Hide();
        P_LoseMenu.Hide();
        P_MainMenu.Show();
    }
    IEnumerator OnStartRoutine(Image background)
    {
        onStartRoutine = null;
        InitialConditions();
        //background from start color to target condition
        yield return StartCoroutine(ColorFade(background, 0f));
    }
    IEnumerator OnLoseRoutine()
    {
        onLoseRoutine = null;
        yield return StartCoroutine(ColorFade(midGameBackgroundImage, 0.3f));
        //P_CollectedCoinInfo.Show();
        yield return new WaitForSecondsRealtime(0.2f);
        P_LoseMenu.Show();
    }
    IEnumerator ChangeSprite()
    {
        while (true)
        {
            tapImage.sprite = isFirst ? tapSprite1 : tapSprite2;
            isFirst = !isFirst;
            yield return new WaitForSeconds(switchTime);
        }
    }

}
