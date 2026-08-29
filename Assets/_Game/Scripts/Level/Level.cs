using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPosition;
    [SerializeField] private CheckpointWall levelCheckpoint;
    [SerializeField] private Transform topMarker;
    [SerializeField] private Transform bottomMarker;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Transform camerasInitialCenter;
    [SerializeField] private float initialRightOffset;
    [SerializeField] private float initialLeftOffset;


    public LevelData levelData;
    public Vector3 SpawnOffset => offset;
    public Vector3Int Origin { get; set; }
    public int SourceIndex { get; set; } = -1;

    public bool IsPlayerInTheLevel { get;  set; }

    public float InitialLeftOffset => initialLeftOffset;
    public float InitialRightOffset => initialRightOffset;
    public CheckpointWall LevelCheckpoint => levelCheckpoint;
    public Transform CameraCenter => camerasInitialCenter;
    public Transform PlayerSpawnPosition => playerSpawnPosition;
    public Transform TopMarker => topMarker;
    public Transform BottomMarker => bottomMarker;

    
}
