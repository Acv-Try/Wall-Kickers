using UnityEngine;
using DG.Tweening;
using System.Collections;
//using Unity.VisualScripting.Dependencies.NCalc;
//using Unity.VisualScripting;


public class CameraController : MonoBehaviour
{

    [SerializeField] private float followingSpeed;
    [SerializeField] private float returningSpeed;
    [SerializeField] private float lookAheadAmount;
    [SerializeField] private float lookAheadLerpSpeed;
    [SerializeField] private GameObject deathLine;
    [SerializeField] private float deathLineOffset;
    public Camera cameraMain;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;
    [SerializeField] private float shakeRandomness;
    private bool canMove = true;
    private bool isReturning;
    private float lastBoxCentreX;
    private float lookAheadOffset;

    private void Start()
    {
        CalcDeadlinePos();
    }
    
    public void SetInitial(Vector3 startCenter)
    {
        transform.position = startCenter;

    }
    private void CalcDeadlinePos()
    {
        float x = deathLine.transform.localPosition.x;
        float y = -deathLineOffset;
        float z = deathLine.transform.localPosition.z;
        deathLine.transform.localPosition = new Vector3(x, y, z);
    }
    public void PauseMovement() => canMove = false;
    public void ResumeMovement() => canMove = true;
    public void StartReturning() => isReturning = true;
    public void HideDeadline() => deathLine.SetActive(false);
    public void ShowDeadline() => deathLine.SetActive(true);
    public void Shake() => transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);

    public void ResetLookAhead()
    {
        lastBoxCentreX = CameraBox.Instance.CenterX;
        lookAheadOffset = 0f;
    }
    //public IEnumerator ReturnToCenter()
    //{
    //    CameraManager.Instance.SetState(CameraState.Returning);
    //    var target = new Vector3(CameraBox.Instance.CenterX, CameraBox.Instance.CenterY, -10f);
    //    while (CameraManager.Instance.State == CameraState.Returning && Vector3.Distance(transform.position, target) > 0.05f)
    //    {
    //        //transform.position = Vector3.Lerp(transform.position, target, returningSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //    if (CameraManager.Instance.State == CameraState.Returning)
    //    {
    //        transform.position = target;
    //        CameraManager.Instance.SetState(CameraState.Frozen);
    //    }

    //}
    public bool HasReachedTarget
    {
        get
        {
            var target = new Vector3(CameraBox.Instance.CenterX, CameraBox.Instance.CenterY, -10f);
            return Vector3.Distance(transform.position, target) < 0.05f;
        }
    }
    private void Update()
    {
        if (!canMove) return;
        LookAheadOffset(CameraBox.Instance.CenterX);
        
        FollowBox(CameraBox.Instance.CenterX, CameraBox.Instance.CenterY);
    }
    private void LookAheadOffset(float centerX)
    {
        float currentBox = centerX;
        float delta = currentBox - lastBoxCentreX;
        lastBoxCentreX = currentBox;

        float targetOffset = Mathf.Abs(delta) > 0.0001f
            ? Mathf.Sign(delta) * lookAheadAmount
            : 0f; // hold last value if box didn't move this frame

        lookAheadOffset = Mathf.Lerp(lookAheadOffset, targetOffset, lookAheadLerpSpeed * Time.deltaTime);

    }
    private void FollowBox(float centerX, float centerY)
    {
        float speed = isReturning ? returningSpeed : followingSpeed;
        float x = centerX + lookAheadOffset;
        float y = centerY;
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(x, y, -10f),
            speed * Time.deltaTime
        );
    }
}