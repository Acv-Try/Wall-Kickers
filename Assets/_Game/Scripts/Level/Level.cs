using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPosition;
    [SerializeField] private CheckPointWall levelCheckpoint;
    [SerializeField] private List<Transform> checkPointsForCamera;
    [SerializeField] private Transform levelCenter;
    [SerializeField] private Transform topMarker;
    [SerializeField] private Transform bottomMarker;
    
    public LevelData levelData;
    
    public Vector3Int Origin { get; set; }
    public Vector3Int MinHeight { get; set; }
    public Vector3Int MaxHeight { get; set; }
    
    public int StartSide { get; set; }
    public int EndSide { get; set; }
    
    public CheckPointWall LevelCheckpoint => levelCheckpoint;
    public Transform LevelCenter => levelCenter;
    public Transform PlayerSpawnPosition => playerSpawnPosition;
    public Transform TopMarker => topMarker;
    public Transform BottomMarker => bottomMarker;
}
