using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LevelManager : MonoBehaviour
{
    //[SerializeField] private List<Level> startLevels;
    [SerializeField] private List<Level> levels;
    [SerializeField] private Transform levelS;
    [SerializeField] private Vector3 initialPosition = Vector3.zero;
    [SerializeField] private Vector3Int initHeightCount;
    [SerializeField] private BackgroundGenerator backgroundGenerator;
    public event Action OnLevelsReady;

    public Vector3 FirstLevelCenter { get; private set; }
    public Vector3 FirstLevelSpawnPos { get; private set; }

    private List<Level> levelsGenerated = new();
    private Transform previousTopMarker;
    private Vector3Int heightCount;
    private int lastLevelFailIndex;
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
    }
    #endregion

    public void Initialize(int lastFailIndex)
    {
        //Debug.Log(lastFailIndex);
        lastLevelFailIndex = lastFailIndex;
        heightCount = initHeightCount;
        backgroundGenerator.Reset();
        for (int i = 0; i < levelsGenerated.Count; i++)
            Destroy(levelsGenerated[i].gameObject);
        levelsGenerated.Clear();

        SpawnNextLevel(0);

        FirstLevelCenter = levelsGenerated[0].LevelCenter.position;
        //Debug.Log(FirstLevelCenter);
        FirstLevelSpawnPos = levelsGenerated[0].PlayerSpawnPosition.position;

        //if (lastLevelFailIndex == 0)
        //    SpawnNextLevel(10);
        //else
        //    SpawnLevel(lastLevelFailIndex);

        OnLevelsReady?.Invoke();
    }

    public void CheckIfPlayerAboveMiddle(Vector3 playerPosition, int currentCheckPoint)
    {
        if (currentCheckPoint < 20) return;
        if (levelsGenerated.Count < 3) return;

        var currentLevel = levelsGenerated[2];
        float middleY = currentLevel.Origin.y + 15;

        if (playerPosition.y > middleY)
        {
            RemoveLevel();
            SpawnNextLevel(currentCheckPoint);
        }
    }

    public void SpawnNextLevel(int checkPoint)
    {
        var level = GetRandomLevelByCheckpoint(checkPoint);
        AddLevel(level);
    }

    public void SpawnLevel(int index)
    {
        var level = levels[index];
        AddLevel(level);
    }

    private void AddLevel(Level level)
    {
        Vector3Int origin = heightCount;

        backgroundGenerator.Paint(level, origin);

        Level instance = Instantiate(
            level,
            new Vector3(0, 0, 0),
            Quaternion.identity,
            levelS
        );

        Vector3 spawnPos = CalculateSpawnPos(instance);

        instance.transform.position = spawnPos;

        heightCount = origin + new Vector3Int(
            level.levelData.markerColumn,
            level.levelData.markerRow,
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

    private Level GetRandomLevelByCheckpoint(int checkpoint)
    {
        int index;

        if (checkpoint < 10) index = 0;
        else if (checkpoint < 50) index = UnityEngine.Random.Range(1, 6);
        else if (checkpoint < 90) index = UnityEngine.Random.Range(6, 10);
        else if (checkpoint < 130) index = UnityEngine.Random.Range(10, 14);
        else if (checkpoint < 170) index = UnityEngine.Random.Range(14, 18);
        else if (checkpoint < 210) index = UnityEngine.Random.Range(18, 22);
        else index = UnityEngine.Random.Range(3, 22);

        index = Mathf.Clamp(index, 0, levels.Count - 1);

        if (checkpoint < 100)
            lastLevelFailIndex = index;
        else
            lastLevelFailIndex = 0;

        var level = levels[index];
        level.LevelCheckpoint.SetCheckPointText(
            Mathf.CeilToInt((checkpoint + 10) / 10f) * 10
        );

        return level;
    }

    public int GetLastFailIndex() => lastLevelFailIndex;
}
