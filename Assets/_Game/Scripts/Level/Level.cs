using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour 
{
    [SerializeField] public Tilemap backgroundTilemap;
    [SerializeField] private List<Transform> checkPointsForCamera;
    [SerializeField] private Transform levelCenter;
    [SerializeField] private Transform spawnPosition;
    public CheckPointWall checkPointWall;
    public LevelData levelData;
    public Vector3Int Origin { get; set; }
    public Vector3Int MinHeight {  get; set; }
    public Vector3Int MaxHeight {  get; set; }
    public int StartSide {  get; set; }
    public int EndSide {  get; set; }
    public Transform LevelCenter => levelCenter;
    public Transform SpawnPosition => spawnPosition;    
}
