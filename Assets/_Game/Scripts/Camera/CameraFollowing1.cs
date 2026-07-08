using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraFollowing1 : MonoBehaviour
{
    [SerializeField] private PlayerController Target;

    [SerializeField] private float Xoffset;
    [SerializeField] private float YOffset;

    [SerializeField] private float speed;
    private Vector3 center = Vector3.negativeInfinity;
    [SerializeField] private GameObject deadLine;

    [SerializeField] private float duration;
    [SerializeField] private float strength;
    [SerializeField] private int vibrato;
    [SerializeField] private float randomness;


    Vector3 NewPosition;

    private bool isCameraFreeze;

    private void OnDestroy()
    {
        Target.OnCameraCheckPointChange -= OnCameraCheckPointChange;
        Target.OnCamerShake -= Shake;
        Target.OnCameraFreeze -= FreezeCamera;

    }
       

    #region
    private static CameraFollowing1 _instance;
    public static CameraFollowing1 Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CameraFollowing1>();
                if (_instance != null)
                {
                    Debug.LogWarning($"CameraFollowing1 is not found in the scene!");
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
        Target.OnCameraCheckPointChange += OnCameraCheckPointChange;
        Target.OnCamerShake += Shake;
        Target.OnCameraFreeze += FreezeCamera;
    }
    #endregion
    public void Initialize()
    {
       
        //center = initialCenter;
        transform.position = new Vector3(center.x, center.y + YOffset, -10);
    }

    void Update()
    {
        if (isCameraFreeze) return;
        if (Target.isCameraMoving)
        {
            var pos = new Vector3(center.x, center.y + YOffset, -10);

            if (Vector3.Distance(transform.position, pos) < 1f)
            {
                Target.isCameraMoving = false;
                deadLine.SetActive(true);
            }
            transform.position = Vector3.Lerp(
                transform.position,
                pos,
                speed * Time.deltaTime
            );
            return;
        }

        float y = 0;
        float x =0;
        if(Target != null)
        {
              y = Mathf.Max(transform.position.y, Target.transform.position.y + YOffset);
            
       
         x = center.x;

        if (Target.transform.position.x > center.x + Xoffset)
            x = Target.transform.position.x - Xoffset / 3;
        else if (Target.transform.position.x < center.x - Xoffset)
            x = Target.transform.position.x + Xoffset / 3;
        }

        NewPosition = new Vector3(x, y, -10);

        transform.position = Vector3.Lerp(
            transform.position,
            NewPosition,
            speed * Time.deltaTime
        );
    }

    public void OnCameraCheckPointChange(Vector3 newPos)
    {
        center = newPos;
        if (Target.isDead)
        {
            StartCoroutine(CameraMoveToInitPos());
        }
    }
    
    private IEnumerator CameraMoveToInitPos()
    {
        FreezeCamera();

        yield return new WaitForSeconds(1f);

        Target.transform.position = Target.spawnPos.position;
        
        Target.playerSprite.enabled = true;
        Target.rb.simulated = true;

        isCameraFreeze = false;
        Target.isDead = false;

        Target.ResetPlayerStats();
    }

    public void Shake()
    {
        transform.DOShakePosition(
            duration,  // duration
            strength,   // strength
            vibrato,      // vibrato
            randomness      // randomness
        );
    }

    public void FreezeCamera()
    {
        isCameraFreeze = true;
        deadLine.SetActive(false);
        Target.isCameraMoving = true;
    }
}