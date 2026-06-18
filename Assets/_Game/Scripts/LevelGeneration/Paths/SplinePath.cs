using UnityEngine;
using UnityEngine.Splines;

public class SplinePath : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;

    [SerializeField] private float speed = 1f;

    private float t;

    private void Update()
    {
        t += speed * Time.deltaTime;

        if (t > 1f)
            t = 1f;

        transform.position = spline.EvaluatePosition(t);
    }
}
