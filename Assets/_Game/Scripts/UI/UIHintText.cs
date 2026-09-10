using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class UIHintText : MonoBehaviour
{
    [SerializeField] private string hintText;
    [SerializeField] private float fadeDuration;
    [SerializeField] private int initialAlpha; //0 or 1
    [TagField]
    [SerializeField] private TagHandle playerTag;
    private TextMeshProUGUI textMesh;
    private int targetAlpha;
    private bool enter = false;
    private Coroutine fadeRoutine;
    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = hintText;
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, initialAlpha);
        targetAlpha = GetTargetAlpha(initialAlpha);
        fadeRoutine = null;

        UIEvents.OnReplay -= OnReplay;
        UIEvents.OnReplay += OnReplay;
    }
    private void OnReplay()
    {
        Debug.Log("enter");
        if (enter == false) return;
        enter = false;
        FadeTo();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enter) return;
        if (collision.gameObject.CompareTag(playerTag.ToString())) return;

        enter = true;
        FadeTo();
    }
    private void FadeTo()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }
        fadeRoutine = StartCoroutine(TextFade());
    }
    private int GetTargetAlpha(int alpha)
    {
        if (alpha == 0) return 1;
        else return 0;
    }
    private IEnumerator TextFade()
    {
        Color start = textMesh.color;
        Color target = new Color(start.r, start.g, start.b, targetAlpha);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            textMesh.color = Color.Lerp(start, target, t);
            yield return null;
        }
        textMesh.color = target;
        targetAlpha = GetTargetAlpha(targetAlpha);
    }
}
