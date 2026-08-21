using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
   
    [SerializeField] private Transform Target; // Reference to the player's transform
    void Update()
    {
       Vector3 NewPosition = new Vector3(Target.position.x, Target.position.y > transform.position.y  ? Target.position.y : transform.position.y, -10); // Update the camera's position to follow the player 
       
       transform.position = Vector3.Lerp(transform.position, NewPosition, Time.deltaTime * 5f);
    }
} //gf