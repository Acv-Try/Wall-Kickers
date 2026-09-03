using System;
using UnityEngine;

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
    //>>>
    [SerializeField] private BaseWall firstWall;
    [SerializeField] private int countPointsPerLevel = 9;


    private bool firstWallOccupied;

    public LevelData levelData;
    
    public event Action<Level> OnFirstWallEntered;
    public Vector3Int Origin { get; set; }
    public int SourceIndex { get; set; } = -1;
    public int CheckpointsCount { get; private set; } = 0;

    public bool IsFirstWallOccupied => firstWallOccupied;
    public bool IsCheckpointSkipped => CheckpointsCount >= countPointsPerLevel;
    public float InitialLeftOffset => initialLeftOffset;
    public float InitialRightOffset => initialRightOffset;
    public BaseWall FirstWall => firstWall;
    public CheckpointWall LevelCheckpoint => levelCheckpoint;
    public Transform CameraCenter => camerasInitialCenter;
    public Transform PlayerSpawnPosition => playerSpawnPosition;
    public Transform TopMarker => topMarker;
    public Transform BottomMarker => bottomMarker;
    public Vector3 SpawnOffset => offset;

    private void OnEnable()
    {
        if (firstWall == null) return;
        firstWall.OnPlayerTouched += HandleFirstWallTouched;
        firstWall.OnPlayerLeft += HandleFirstWallLeft;
    }
    private void OnDisable()
    {
        if (firstWall == null) return;
        firstWall.OnPlayerTouched -= HandleFirstWallTouched;
        firstWall.OnPlayerLeft -= HandleFirstWallLeft;
    }
    public void IncreaseCount()
    {
        CheckpointsCount++;
    }
    public void ResetCheckpointsCount() { CheckpointsCount = 0; }
    public void ClearFirstWallSignal() => firstWallOccupied = false;

    private void HandleFirstWallTouched(PlayerController player)
    {
        firstWallOccupied = true;
        OnFirstWallEntered?.Invoke(this);
     }
    private void HandleFirstWallLeft(PlayerController player) => firstWallOccupied = false;
    
}
