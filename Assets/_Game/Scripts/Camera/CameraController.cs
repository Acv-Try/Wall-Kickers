using UnityEngine;
using DG.Tweening;
using System.Collections;
//using Unity.VisualScripting.Dependencies.NCalc;
//using Unity.VisualScripting;


public class CameraController : MonoBehaviour
{

    [SerializeField] private float followingSpeed;
    [SerializeField] private float returningSpeed;
    [SerializeField] private GameObject deathLine;
    [SerializeField] private float deathlineOffset;
    public Camera cameraMain;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;
    [SerializeField] private float shakeRandomness;
    private bool canMove = true;

    public void PauseMovement() => canMove = false;
    public void ResumeMovement() => canMove = true;

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
        float y = -deathlineOffset;
        float z = deathLine.transform.localPosition.z;
        deathLine.transform.localPosition = new Vector3(x, y, z);
    }

    public void HideDeadline() => deathLine.SetActive(false);
    public void ShowDeadline() => deathLine.SetActive(true);
    public void Shake() => transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);

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
        float x = CameraBox.Instance.CenterX;
        float y = CameraBox.Instance.CenterY;
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(x, y, -10f),
            followingSpeed * Time.deltaTime
        );
    }
}