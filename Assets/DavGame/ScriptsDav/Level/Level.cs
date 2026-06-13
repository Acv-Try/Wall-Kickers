using UnityEngine;
using System.Collections.Generic;


public class Level : MonoBehaviour 
{
    [SerializeField] private List<Transform> checkPointsForCamera;

    public LevelData levelData;
    public Vector3Int Origin { get; set; }
    public int MinHeightY {  get; set; }
    public int MaxHeightY {  get; set; }
    public int StartSide {  get; set; }
    public int EndSide {  get; set; }

    public Transform GetCheckPointForCamera()
    {
        var check = checkPointsForCamera[0];
        checkPointsForCamera.RemoveAt(0);
        return check;
    }
}
