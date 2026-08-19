using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class LevelManager : MonoBehaviour
{
    //[SerializeField] private List<Level> startLevels;
    [SerializeField] private List<Level> startLevels;
    [SerializeField] private List<Level> levels;
    [SerializeField] private Transform levelS;
    [SerializeField] private Vector3 initialPosition = Vector3.zero;
    [SerializeField] private Vector3Int initHeightCount;
    [SerializeField] private BackgroundGenerator backgroundGenerator;
    public event Action OnLevelsReady;

    public Vector3 FirstLevelCenter { get; private set; }
    public Vector3 FirstLevelSpawnPos { get; private set; }
    public int LocalLevelCheckpoints { get; set; }
    public int TotalCheckpoints { get; set; }


    private List<Level> levelsGenerated = new List<Level>(3);
    private List<int> unusedLevels = new();
    private Transform previousTopMarker;
    private Vector3Int heightCount;

    private int currentLevelIndex;
    private int nextCheckpointValue;
    #region
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<LevelManager>();
                if (_instance == null)
                {
                    Debug.LogWarning($"LevelManager is not found in the scene!");
                }
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
        PlayerManager.Instance.OnPlayerPositionChange -= CheckIfPlayerAboveMiddle;
        PlayerManager.Instance.OnPlayerPositionChange += CheckIfPlayerAboveMiddle;
    }
    #endregion

    public void Initialize()
    {
        //Debug.Log(lastFailIndex);
        heightCount = initHeightCount;
        backgroundGenerator.Reset();
        for (int i = 0; i < levelsGenerated.Count; i++)
            Destroy(levelsGenerated[i].gameObject);
        levelsGenerated.Clear();

        RefillUnusedLevels();
        nextCheckpointValue = 10;

        SpawnStartLevel();

        //FirstLevelCenter = levelsGenerated[0].LevelCenter.position;
        FirstLevelSpawnPos = levelsGenerated[0].PlayerSpawnPosition.position;

        //if (lastLevelFailIndex == 0)
        //    SpawnNextLevel(10);
        //else
        //    SpawnLevel(lastLevelFailIndex);
        for (int i = 0; i < 3; i++)
        {
            SpawnNextLevel();
        }

        OnLevelsReady?.Invoke();
    }

    public void CheckIfPlayerAboveMiddle(Vector3 playerPosition)
    {
        int currentCheckpoint = TotalCheckpoints;

        if (currentCheckpoint < 20) return;
        if (levelsGenerated.Count < 3) return;

        var middleLevel = levelsGenerated[1]; // 2nd of the 3 currently drawn
        float middleYAxis = (middleLevel.BottomMarker.position.y + middleLevel.TopMarker.position.y) * 0.5f;
        if(playerPosition.y > middleYAxis)
        {
            RemoveLevel();
            SpawnNextLevel();
        }
    }

    public void SpawnStartLevel()
    {
        var level = GetRandomStartLevel();
        AddLevel(level);
    }
    public void SpawnNextLevel()
    {

        var level = GetRandomLevel();
        AddLevel(level);
    }
    private void RefillUnusedLevels()
    {
        unusedLevels.Clear();

        for (int i = 0; i < levels.Count; i++)
            unusedLevels.Add(i);
    }
    private void AddLevel(Level level)
    {
        Vector3Int origin = heightCount - new Vector3Int(
            level.levelData.bottomMarkerColumn,
            level.levelData.bottomMarkerRow,
            0);

        backgroundGenerator.Paint(level, origin);
        Debug.Log(backgroundGenerator.CellToWorld(origin));
        Vector3 worldOrigin = backgroundGenerator.CellToWorld(origin) + new Vector3(1f, 0, 0);
        Level instance = Instantiate(
            level,
            worldOrigin,
            Quaternion.identity,
            levelS
        );

        //Vector3 spawnPos = CalculateSpawnPos(instance);

        //instance.transform.position = spawnPos;

        heightCount = origin + new Vector3Int(
            level.levelData.topMarkerColumn,
            level.levelData.topMarkerRow,
            0);

        levelsGenerated.Add(instance);
    }
    private Vector3 CalculateSpawnPos(Level level)
    {
        if (previousTopMarker == null)
        {
            Vector2 initPos = initialPosition - level.BottomMarker.localPosition;
            previousTopMarker = level.TopMarker;
            return initPos;
        }

        Vector3 nextPos = previousTopMarker.position - level.BottomMarker.localPosition;
        previousTopMarker = level.TopMarker;
        return nextPos;

    }
    private void RemoveLevel()
    {
        var level = levelsGenerated[0];
        levelsGenerated.RemoveAt(0);

        backgroundGenerator.Remove(level);

        Destroy(level.gameObject);
    }

    private Level GetRandomLevel()
    {
        if (unusedLevels.Count == 0)
            RefillUnusedLevels();

        int pick = Random.Range(0, unusedLevels.Count);
        int levelIndex = unusedLevels[pick];
        unusedLevels.RemoveAt(pick);

        var level = levels[levelIndex];
        return level;

        //int index = Random.Range(levelIndex, levels.Count);
        //index = Mathf.Clamp(index, levelIndex, levels.Count - 1);
        //level.LevelCheckpoint.SetCheckPointText(Mathf.CeilToInt(levelIndex * 10));
    }
    private Level GetRandomStartLevel()
    {
        int index = Random.Range(0, startLevels.Count);
        index = Mathf.Clamp(index, 0, startLevels.Count - 1);
        var level = startLevels[index];
        level.LevelCheckpoint.SetCheckPointText(nextCheckpointValue);
        return level;

    }
    public void ComputeCurrentLevelIndex()
    {
        foreach (var level in levelsGenerated)
        {
            if (level.IsPlayerInTheLevel == true)
            {
                currentLevelIndex = levels.IndexOf(level);
            }
        }

    }
    public void IncreaseCheckpoint()
    {
        TotalCheckpoints++;
    }
}
