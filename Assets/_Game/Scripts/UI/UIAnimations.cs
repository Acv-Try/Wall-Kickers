using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIAnimations : MonoBehaviour
{
    [SerializeField] private Image tapImage;
    [SerializeField] private Sprite tapSprite1;
    [SerializeField] private Sprite tapSprite2;
    [SerializeField] private Image muteImage;
    [SerializeField] private Sprite muteSprite1;
    [SerializeField] private Sprite muteSprite2;
    [SerializeField] private Image NotifImage;
    [SerializeField] private Sprite NotifSprite1;
    [SerializeField] private Sprite NotifSprite2;
    [SerializeField] private float switchTime = 1f;
    [SerializeField] private UIPanelSlider topPanel, bottomPanel;

    private bool isFirst = true;
    private bool isMute = true;
    private bool isNotif = true;

    private void Start()
    {
        StartCoroutine(ChangeSprite());
    }

    public void OnClickMuteButton()
    {
        muteImage.sprite = isFirst ? muteSprite1 : muteSprite2;
        isMute = !isMute;
    }

    public void OnClickNotifButton()
    {
        NotifImage.sprite = isFirst ? NotifSprite1 : NotifSprite2;
        isNotif = !isNotif;
    }
    public void OnGameStart()
    {
        Debug.Log("Enter show");
        topPanel.Show();
        bottomPanel.Show();
    }
    public void OnPlayerClick()
    {
        Debug.Log("Enter hide");
        topPanel.Hide();
        bottomPanel.Hide();
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
