using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CanvasTapAnimation : MonoBehaviour
{
    [SerializeField] private Image tapImage;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private float switchTime = 1f;

    private bool isFirst = true;

    private void Start()
    {
        StartCoroutine(ChangeSprite());
    }

    IEnumerator ChangeSprite()
    {
        while (true)
        {
            tapImage.sprite = isFirst ? sprite1 : sprite2;
            isFirst = !isFirst;
            yield return new WaitForSeconds(switchTime);
        }
    }
}
