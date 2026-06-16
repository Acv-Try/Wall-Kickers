using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    float YHigh;
    public int Score;
    void Update()
    {
        if(transform.position.y > YHigh + 3)
        {
            YHigh = transform.position.y;
            Score++;

            if(Score % 10 == 0)
            {
                Debug.Log("NewLvL");
            }
        }
    }
}
