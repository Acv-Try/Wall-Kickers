using UnityEngine;
using System.Collections;
public class DisappearingWall : BaseWall
{
    public float TimeToRespawn = 4f;
    [SerializeField] private string crackAnimaName;
    [SerializeField] private string fallApartAnimaName;
    [SerializeField] private float fadeDuration;

    private Collider2D col;
    private SpriteRenderer sr;
    private Coroutine colorRoutine, timerRoutine;
    private void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        Initialize();
    }
    public override void Left(PlayerController player)
    {
        timerRoutine = StartCoroutine(Timer());
    }

    public override void Touched(PlayerController player)
    {
        _animator.SetTrigger(crackAnimaName);
    }

    private void ChangeState(bool state)
    {
        col.enabled = state;
        //sr.enabled = state;
    }

    //call in the animation event
    public void OnDisappearing()
    {
        ChangeState(false);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a * 0);
    }
    private IEnumerator Timer()
    {
        _animator.SetTrigger(fallApartAnimaName);
        col.enabled = false;
        yield return new WaitForSeconds(TimeToRespawn);
        _animator.SetTrigger("SetIdle");
        ChangeState(true);
        yield return colorRoutine = StartCoroutine(ColorFade());
        timerRoutine = null;
    }

    private IEnumerator ColorFade()
    {
        Color startColor = sr.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1);
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;
            sr.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        sr.color = targetColor;
        colorRoutine = null;
    }
}