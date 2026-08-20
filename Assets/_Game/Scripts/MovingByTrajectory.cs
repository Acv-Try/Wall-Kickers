using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
public class MovingByTrajectory : MonoBehaviour
{
     [SerializeField] private SplineContainer spline;
    [SerializeField] private Rigidbody2D rb_mov;
    [SerializeField] private float coolDownTime;
    [SerializeField] private float speed;
    float t = 0f;
    int dir = 1;
    bool isCooldown;

    void Update()
    {
        if (isCooldown) return;

        t += dir * speed * Time.deltaTime;

        if (t >= 1f)
        {
            t = 1f;
            dir = -1;
            StartCoroutine(CoolDown());
        }
        else if (t <= 0f)
        {
            t = 0f;
            dir = 1;
            StartCoroutine(CoolDown());
        }

        float easedT = Mathf.SmoothStep(0f, 1f, t);

        Vector3 pos = spline.EvaluatePosition(easedT);

        transform.position = pos;
    }

    IEnumerator CoolDown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(coolDownTime);
        isCooldown = false;
    }
}
