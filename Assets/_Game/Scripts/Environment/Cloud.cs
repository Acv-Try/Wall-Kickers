using System.Xml.Schema;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float randomYRange = 100f;

    private float leftEdge;
    private float rightEdge;
    private float cloudWidth;

    private void Start()
    {
        var canvas = GetComponentInParent<Canvas>();
        var rect = canvas.GetComponent<RectTransform>().rect;

        leftEdge = rect.xMin;
        rightEdge = rect.xMax;

        cloudWidth = GetComponent<RectTransform>().rect.width;

        // Randomize starting X so clouds don't all start at the same point
        transform.localPosition = new Vector3(
            Random.Range(leftEdge, rightEdge),
            transform.localPosition.y,
            transform.localPosition.z
        );
    }

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // When cloud fully exits left edge, wrap to right edge at random Y
        if (transform.localPosition.x < leftEdge - cloudWidth)
        {
            transform.localPosition = new Vector3(
                rightEdge + cloudWidth,
                Random.Range(-randomYRange, randomYRange),
                transform.localPosition.z
            );
        }
    }
}
