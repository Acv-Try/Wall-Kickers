using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Level> levels;
    [SerializeField] private Vector3Int initHeightCount;

    public event Action<Vector3, Vector3> OnFirstLevelReady;

    private List<Level> levelsGenerated = new();
    private Vector3Int heightCount;
    private int lastLevelFailIndex;
    private Level firstLevel;

    #region
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<LevelManager>();
                if (_instance != null)
                {
                    Debug.LogWarning($"LevelManager is not found in the scene!");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerManager.Instance.OnCheckpointReached += OnCheckpointReached;
    }
    #endregion

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerManager.Instance.OnCheckpointReached -= OnCheckpointReached;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        heightCount = initHeightCount;
        BackgroundGenerator.Instance.ClearAll();

        for (int i = 0; i < levelsGenerated.Count; i++)
            Destroy(levelsGenerated[i].gameObject);
        levelsGenerated.Clear();

        GenerateNextLevel(0);
        firstLevel = levelsGenerated[0];

        if (lastLevelFailIndex == 0)
            GenerateNextLevel(10);
        else
            GenerateLevelFromIndex(lastLevelFailIndex);


        OnFirstLevelReady?.Invoke(
            firstLevel.LevelCenter.position,
            firstLevel.SpawnPosition.position
        );
    }

    private void OnCheckpointReached(int total, int countInLevel)
    {
        //UIManager.Instance.UpdateGameplayScore(total);
        CheckIfPlayerAboveMiddle(PlayerManager.Instance.Status.transform.position, total);
    }

    private void CheckIfPlayerAboveMiddle(Vector3 playerPosition, int currentCheckPoint)
    {
        if (currentCheckPoint < 20) return;

        var currentLevel = levelsGenerated[2];
        float middleY = currentLevel.Origin.y + 15;

        if (playerPosition.y > middleY)
        {
            RemoveLevel();
            GenerateNextLevel(currentCheckPoint);
        }
    }

    public void GenerateNextLevel(int checkPoint)
    {
        var level = GetRandomLevelByCheckpoint(checkPoint);
        GenerateLevel(level);
    }

    public void GenerateLevelFromIndex(int index)
    {
        var level = levels[index];
        GenerateLevel(level);
    }

    private void GenerateLevel(Level level)
    {
        var edges = BackgroundGenerator.Instance.Generate(level, heightCount);

        Vector3Int spawnPos = heightCount - edges.min;
        var instance = Instantiate(
            level,
            (Vector3)spawnPos,
            Quaternion.identity,
            BackgroundGenerator.Instance.transform
        );

        instance.MinHeight = edges.min;
        instance.MaxHeight = edges.max;
        instance.Origin = heightCount;

        levelsGenerated.Add(instance);
        heightCount += new Vector3Int(
            instance.MaxHeight.x - instance.MinHeight.x,
            instance.MaxHeight.y - instance.MinHeight.y + 1,
            0
        );
    }

    private void RemoveLevel()
    {
        var level = levelsGenerated[0];
        levelsGenerated.RemoveAt(0);
        BackgroundGenerator.Instance.Clear(level);
        Destroy(level.gameObject);
    }

    private Level GetRandomLevelByCheckpoint(int checkPoint)
    {
        int index;

        if (checkPoint < 10) index = 0;
        else if (checkPoint < 50) index = UnityEngine.Random.Range(1, 6);
        else if (checkPoint < 90) index = UnityEngine.Random.Range(6, 10);
        else if (checkPoint < 130) index = UnityEngine.Random.Range(10, 14);
        else if (checkPoint < 170) index = UnityEngine.Random.Range(14, 18);
        else if (checkPoint < 210) index = UnityEngine.Random.Range(18, 22);
        else index = UnityEngine.Random.Range(3, 22);

        index = Mathf.Clamp(index, 0, levels.Count - 1);

        if (checkPoint < 100)
            lastLevelFailIndex = index;
        else
            lastLevelFailIndex = 0;

        var level = levels[index];
        level.checkPointWall.SetCheckPointText(
            Mathf.CeilToInt((checkPoint + 10) / 10f) * 10
        );

        return level;
    }
}
