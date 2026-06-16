using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScriptForMoveObstacle : MonoBehaviour
{

    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed = 2f;
    [SerializeField] float WaitTime = 1f;
    [SerializeField] bool StopOnFirstAndOnLastPoint = true;
    [SerializeField] bool MoveByCurve = false;

    Rigidbody2D rb;

    int currentWaypoint = 0;
    void Start()
{
    rb = GetComponent<Rigidbody2D>();
    
    if(MoveByCurve && waypoints.Length >= 3)
    {
    GenerateCurvePoints();
    }
    else
    {
        foreach(Transform waypoint in waypoints)
        {
            Points.Add(waypoint.position);
        }
    }
}

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;       
        MovePlatform();      
    }

    bool IsMovingForward = true;
    void MovePlatform()
    {
        Vector2 target = Points[currentWaypoint];
        
        if(!IsWaiting)
        {
        rb.MovePosition((target - rb.position).normalized * speed * Time.deltaTime + rb.position);

            if(Vector2.Distance(transform.position, target) < 0.05f)
            {     
                 if(StopOnFirstAndOnLastPoint)
                {             
                  if(target == Points[Points.Count - 1] || target == Points[0])
                  {               
                
                    IsWaiting = true;
                    StartCoroutine(StopAndWait());
      
                    if(target == Points[Points.Count - 1])
                    {
                        IsMovingForward = false;
                    }
                    else
                    {
                        IsMovingForward = true;
                    }   
                  }

                     if(IsMovingForward)
                   {
                       currentWaypoint++;
                   }
                   else 
                   {
                       currentWaypoint--;
                   }
                 
                }
                else
                {
                    if(currentWaypoint == Points.Count - 1)
                    {
                        currentWaypoint = 0;
                    }
                    else
                    {
                        currentWaypoint++;
                    }
                }

            }      
        }
    } 
    bool IsWaiting = false;
    IEnumerator StopAndWait()
    {
        float originalSpeed = speed;
        speed = 0f;
        IsWaiting = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(WaitTime);
        speed = originalSpeed;
        IsWaiting = false;
    }

    public List<Vector2> Points = new List<Vector2>();

    void GenerateCurvePoints()
{
    Points.Clear();

    int resolution = 25;

    for(int i = 0; i <= resolution; i++)
    {
        float t = i / (float)resolution;

        Vector2 pos =
            Mathf.Pow(1 - t, 2) * (Vector2)waypoints[0].position +
            2 * (1 - t) * t * (Vector2)waypoints[1].position +
            Mathf.Pow(t, 2) * (Vector2)waypoints[2].position;

        Points.Add(pos);
    }
}
}
