using UnityEngine;

public class HalfWidth : MonoBehaviour
{
    public void SetXPos(float newX)
    {
        float x = (transform.position.x * 0) + newX;
        float y = transform.localPosition.y;
        float z = transform.localPosition.z;
        transform.localPosition = new Vector3(x,y,z);
    }
}
