using System.Xml.Schema;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed;
    private float leftEdge;
    private float rightEdge;
    private float downEdge;
    private float upEdge;

    private void Start()
    {
        var rect = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
        leftEdge = rect.xMin;
        rightEdge = rect.xMax;

        downEdge = rect.yMin;
        upEdge = rect.yMax;

    }

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.localPosition.x < leftEdge)
        {
            transform.localPosition = new Vector3(rightEdge, Random.Range(downEdge, upEdge), transform.localPosition.z);
        }
        //Debug.Log(downEdge + " " + upEdge + " " + leftEdge + " " + rightEdge);

    }
}
