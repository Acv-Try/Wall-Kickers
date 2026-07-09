using System.Collections;
using UnityEngine;

public class UIPanelSlider : MonoBehaviour
{

    [SerializeField] private RectTransform _rect;
    [SerializeField] private Vector2 _hiddenPosition;
    [SerializeField] private Vector2 _shownPosition;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private AnimationCurve _ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _activeRoutine;

    private void Reset()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Show() => PlayTo(_shownPosition);
    public void Hide() => PlayTo(_hiddenPosition);

    public void PlayTo(Vector2 target)
    {
        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        _activeRoutine = StartCoroutine(AnimateTo(target));
    }

    private IEnumerator AnimateTo(Vector2 target)
    {
        Vector2 start = _rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = _ease.Evaluate(Mathf.Clamp01(elapsed / _duration));
            _rect.anchoredPosition = Vector2.LerpUnclamped(start, target, t);
            yield return null;
        }

        _rect.anchoredPosition = target;
        _activeRoutine = null;
    }
}

