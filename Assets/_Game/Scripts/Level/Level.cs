using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Level : MonoBehaviour 
{
    [SerializeField] private List<Transform> checkPointsForCamera;
    public CheckPointWall checkPointWall;

    public LevelData levelData;
    public Vector3Int Origin { get; set; }
    public Vector3Int MinHeight {  get; set; }
    public Vector3Int MaxHeight {  get; set; }
    public int StartSide {  get; set; }
    public int EndSide {  get; set; }

    public Transform GetCheckPointForCamera()
    {
        var check = checkPointsForCamera[0];
        checkPointsForCamera.RemoveAt(0);
        return check;
    }

    public IEnumerable<Vector3> GetNextCheckPointOfCamera()
    {
        foreach (var point in checkPointsForCamera)
        {
            yield return point.position;
        }
    }
}
