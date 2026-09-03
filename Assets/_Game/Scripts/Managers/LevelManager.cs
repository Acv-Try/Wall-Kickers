using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Level> startLevels;
    [SerializeField] private List<Level> levels;
    [SerializeField] private Vector3Int initHeightCount;

    private LevelGenerator levelGenerator;
    private readonly CheckpointSkipDetector skipDetector = new CheckpointSkipDetector();
    public event Action OnLevelsReady;

    public Vector3 FirstLevelCameraCenter { get; private set; }
    public float FirstLevelLeftOffset { get; private set; }
    public float FirstLevelRightOffset { get; private set; }
    public Vector3 FirstLevelSpawnPos { get; private set; }
    public int TotalCheckpoints { get; set; }

    private const int WindowSize = 3;

    private List<Level> generated = new List<Level>(WindowSize);
    private List<int> unused = new();
    private Vector3Int heightCount;
    private int nextCheckpointValue;

    #region Singleton
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<LevelManager>();
                if (_instance == null)
                    Debug.LogWarning($"LevelManager is not found in the scene!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        levelGenerator = GetComponentInChildren<LevelGenerator>();
        PlayerManager.Instance.OnPlayerPositionChange -= CheckIfPlayerAboveMiddle;
        PlayerManager.Instance.OnPlayerPositionChange += CheckIfPlayerAboveMiddle;
    }
    #endregion

    public void Initialize(int forcedLevelIndex = -1)
    {
        levelGenerator.Reset();
        heightCount = initHeightCount;

        for (int i = 0; i < generated.Count; i++)
            Destroy(generated[i].gameObject);
        generated.Clear();

        RefillUnusedLevels();
        TotalCheckpoints = 0;
        nextCheckpointValue = 10;

        SpawnStartLevel();

        FirstLevelSpawnPos = generated[0].PlayerSpawnPosition.position;

        FirstLevelCameraCenter = generated[0].CameraCenter.position;

        FirstLevelLeftOffset = generated[0].InitialLeftOffset;
        FirstLevelRightOffset = generated[0].InitialRightOffset;

        for (int i = 1; i < WindowSize; i++)
        {
            if (i == 1 && forcedLevelIndex >= 0)
            {
                unused.Remove(forcedLevelIndex);
                var forced = levels[forcedLevelIndex];
                nextCheckpointValue += 10;
                forced.LevelCheckpoint.SetCheckPointText(nextCheckpointValue);
                AddLevel(forced, forcedLevelIndex);
            }
            else
                SpawnNextLevel();
        }

        OnLevelsReady?.Invoke();
    }
    public void OnPlayerDeath()
    {
        foreach (var level in generated)
        {
            level.ResetCheckpointsCount();
        }
    }

    public void CheckIfPlayerAboveMiddle(Vector3 playerPosition)
    {
        if (generated.Count < WindowSize) return;
        if (TotalCheckpoints < 20) return;
        var middleLevel = generated[1]; // the level the player is currently on
        float middleYAxis = (middleLevel.BottomMarker.position.y + middleLevel.TopMarker.position.y) * 0.5f;

        if (playerPosition.y > middleYAxis)
        {
            RemoveOldestLevel();
            SpawnNextLevel();
        }
    }
    private void HandleLevelFirstWallEntered(Level current)
    {
        int index = generated.IndexOf(current);
        if (index <= 0) return; // no previous level to check against
        skipDetector.Evaluate(generated[index - 1], current, PlayerManager.Instance.PlayerTransform.position);
    }
    private void SpawnStartLevel()
    {
        var level = GetRandomStartLevel();
        AddLevel(level);
    }

    private void SpawnNextLevel()
    {
        if (unused.Count == 0)
            RefillUnusedLevels();

        int pick = Random.Range(0, unused.Count);
        int levelIndex = unused[pick];
        unused.RemoveAt(pick);

        var level = levels[levelIndex];
        nextCheckpointValue += 10;
        level.LevelCheckpoint.SetCheckPointText(nextCheckpointValue);

        AddLevel(level, levelIndex);
    }

    private void AddLevel(Level level, int sourceIndex)
    {
        Vector3Int origin = heightCount - new Vector3Int(
            level.levelData.bottomMarkerColumn,
            level.levelData.bottomMarkerRow,
            0);

        Level instance = levelGenerator.SpawnLevel(level, origin);
        instance.SourceIndex = sourceIndex;

        heightCount = origin + new Vector3Int(
            level.levelData.topMarkerColumn,
            level.levelData.topMarkerRow,
            0);
        instance.OnFirstWallEntered += HandleLevelFirstWallEntered;
        generated.Add(instance);
    }
    private void AddLevel(Level level) => AddLevel(level, -1);
    private void RemoveOldestLevel()
    {
        if (generated.Count == 0) return;
        var oldest = generated[0];
        oldest.OnFirstWallEntered -= HandleLevelFirstWallEntered;
        generated.RemoveAt(0);
        levelGenerator.RemoveLevel(oldest);
    }

    private void RefillUnusedLevels()
    {
        unused.Clear();
        for (int i = 0; i < levels.Count; i++)
            unused.Add(i);
    }

    private Level GetRandomLevel()
    {
        if (unused.Count == 0)
            RefillUnusedLevels();

        int pick = Random.Range(0, unused.Count);
        int levelIndex = unused[pick];
        unused.RemoveAt(pick);

        var level = levels[levelIndex];
        nextCheckpointValue += 10;
        level.LevelCheckpoint.SetCheckPointText(nextCheckpointValue);
        return level;
    }

    private Level GetRandomStartLevel()
    {
        int index = Random.Range(0, startLevels.Count);
        var level = startLevels[index];
        level.LevelCheckpoint.SetCheckPointText(nextCheckpointValue);
        return level;
    }
    public int GetCurrentLevelSourceIndex() => generated.Count >= 2 ? generated[1].SourceIndex : -1;
    public void IncreaseCheckpoint() => TotalCheckpoints++;
}