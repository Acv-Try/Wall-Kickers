using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPosition;
    [SerializeField] private CheckPointWall levelCheckpoint;
    [SerializeField] private Transform levelCenter;
    [SerializeField] private Transform topMarker;
    [SerializeField] private Transform bottomMarker;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private List<Transform> checkPointsForCamera;
    
    public LevelData levelData;
    public Vector3 SpawnOffset => offset;
    public Vector3Int Origin { get; set; }
    public Vector3Int MinHeight { get; set; }
    public Vector3Int MaxHeight { get; set; }
    
    public int StartSide { get; set; }
    public int EndSide { get; set; }

    public bool IsPlayerInTheLevel { get;  set; }

    public CheckPointWall LevelCheckpoint => levelCheckpoint;
    public Transform LevelCenter => levelCenter;
    public Transform PlayerSpawnPosition => playerSpawnPosition;
    public Transform TopMarker => topMarker;
    public Transform BottomMarker => bottomMarker;

    
}
