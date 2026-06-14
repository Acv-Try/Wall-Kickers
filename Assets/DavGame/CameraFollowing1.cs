using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraFollowing1 : MonoBehaviour
{
    [SerializeField] private Player Target;

    [SerializeField] private float Xoffset;
    [SerializeField] private float YOffset;

    [SerializeField] private float speed;
    private Vector3 center = Vector3.negativeInfinity;
    [SerializeField] private GameObject deadLine;

    Vector3 NewPosition;

    private bool isCameraFreeze;
    private void Awake()
    {
        Target.OnCameraCheckPointChange += OnCameraCheckPointChange;
    }

    private void OnDestroy()
    {
        Target.OnCameraCheckPointChange -= OnCameraCheckPointChange;
    }


    private void Start()
    {
        transform.position = new Vector3(center.x, center.y + YOffset, -10);
        //Debug.DrawLine(
        //    new Vector3(center.x + Xoffset, center.y - 100f, center.z),
        //    new Vector3(center.x + Xoffset, center.y + 100f, center.z),
        //    Color.red,
        //    10f
        //);

        //Debug.DrawLine(
        //    new Vector3(center.x - Xoffset, center.y - 100f, center.z),
        //    new Vector3(center.x - Xoffset, center.y + 100f, center.z),
        //    Color.red,
        //    10f
        //);
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

        float y = Mathf.Max(transform.position.y, Target.transform.position.y + YOffset);

        float x = center.x;

        if (Target.transform.position.x > center.x + Xoffset)
            x = Target.transform.position.x - Xoffset / 3;
        else if (Target.transform.position.x < center.x - Xoffset)
            x = Target.transform.position.x + Xoffset / 3;

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
        //Debug.Log("Wait");
        isCameraFreeze = true;
        deadLine.SetActive(false);

        yield return new WaitForSeconds(1f);

        isCameraFreeze = false;
        Target.isDead = false;
        Target.isCameraMoving = true;
        Target.ResetPlayerStats();
    }
}