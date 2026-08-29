using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public partial class GridGenerator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI score;
    private static GridGenerator instance;
    public static GridGenerator Instance => instance;
    //[Header("Rule Tiles")]
    //[SerializeField] private RuleTile NormalWallRuleTile;

    [Header("Lists")]
    [SerializeField] private List<Level> levels;

    [SerializeField] private GameObject grid;
    [SerializeField] private int heightOffset;
    [SerializeField] private Vector3Int InitHeightCount;
    [SerializeField] private Transform initSpawnPos;
    
    private List<Level> levelsGenerated = new();
    private Vector3Int heightCount;
    private int lastLevelFailIndex;
    private Level currentLevel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public Vector3 GetFirstLevelCenter()
    {
        return currentLevel.CameraCenter.position;
    }
    public Vector3 GetSpawnPosition()
    {
        return currentLevel.PlayerSpawnPosition.position;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        heightCount = InitHeightCount;
        score.text = "0";
        background.ClearAllTiles();

        for (int i = 0; i < levelsGenerated.Count; i++)
        {
            Destroy(levelsGenerated[i].gameObject);
        }
        levelsGenerated.Clear();

        GenerateNextLevel(0);
        currentLevel = levelsGenerated[0];
        if (lastLevelFailIndex == 0)
        {
            Debug.Log("enter if");
            GenerateNextLevel(10);

        }
        else
        {
            Debug.Log("enter else");
            GenerateLevelFromIndexOfLevel(lastLevelFailIndex);
        }
        //GenerateNextLevel(20);

    }

    public void GenerateNextLevel(int checkPoint)
    {
        var level = GetRandomLevelFromCheckPoint(checkPoint);
        GenerateLevel(level);

    }

    public void GenerateLevelFromIndexOfLevel(int indexLevel)
    {
        var level = levels[indexLevel];
        GenerateLevel(level);
    }

    public void GenerateLevel(Level level)
    {
        (Vector3Int minH, Vector3Int maxH) edges = GenerateBackground(level);

        //Vector3Int spawnPos = heightCount - edges.Item1;
        //var levelInstance = Instantiate(level, spawnPos, Quaternion.identity, transform);
        Vector3 spawnPos = edges.Item1 / 2;
        var levelInstance = Instantiate(level, spawnPos, Quaternion.identity, transform);

        //levelInstance.MaxHeight = edges.Item2;
        //levelInstance.MinHeight = edges.Item1;
        levelInstance.Origin = heightCount;

        levelsGenerated.Add(levelInstance);
        //heightCount += new Vector3Int(
            //levelInstance.MaxHeight.x - levelInstance.MinHeight.x, 
            //levelInstance.MaxHeight.y - levelInstance.MinHeight.y + 1, 0);
    }

    public Level GetRandomLevelFromCheckPoint(int checkPoint)
    {
        int index = 0;
        if (checkPoint < 10) index = 0;
        else if (checkPoint >= 10 && checkPoint < 50) index = UnityEngine.Random.Range(1, 6);
        else if (checkPoint >= 50 && checkPoint < 90) index = UnityEngine.Random.Range(6, 10);
        else if (checkPoint >= 90 && checkPoint < 130) index = UnityEngine.Random.Range(10, 14);
        else if (checkPoint >= 130 && checkPoint < 170) index = UnityEngine.Random.Range(14, 18);
        else if (checkPoint >= 170 && checkPoint < 210) index = UnityEngine.Random.Range(18, 22);
        else index = UnityEngine.Random.Range(3, 22);

        index = Mathf.Clamp(index, 0, levels.Count - 1);

        var level = levels[index];
        if (checkPoint < 100)
            lastLevelFailIndex = index;
        else
            lastLevelFailIndex = 0;
        level.LevelCheckpoint.SetCheckPointText(Mathf.CeilToInt((checkPoint + 10) / 10f) * 10);
        return level;
    }

    public void RemoveLevel()
    {
        //var level = levelsGenerated[0];
        //levelsGenerated.RemoveAt(0);
        //BoundsInt bounds = new BoundsInt(
        //    level.Origin.x,
        //    level.Origin.y,
        //    level.Origin.z,
        //    level.levelData.columns,
        //    level.MaxHeight.y,
        //    1
        //);
        //Debug.Log(bounds.x + " " + bounds.y + " " + bounds.z);
        //foreach (var pos in bounds.allPositionsWithin)
        //{
        //    background.SetTile(pos, null);
        //}

        //Destroy(level.gameObject);
    }

    public void CheckIfPlayerAboveOfMiddleLevel(Vector3 playerPosition, int currentCheckPoint)
    {
        // UI 
        AddScore(currentCheckPoint);

        // Dont generate level on First Level
        if (currentCheckPoint < 20) return;

        currentLevel = levelsGenerated[2];
        //Debug.Log(currentLevel.Origin + " " + currentCheckPoint);
        float middleY = currentLevel.Origin.y + 15;

        if (playerPosition.y > middleY)
        {
            Debug.Log("Pos" + playerPosition.y + " Mid " + middleY);
            RemoveLevel();
            Debug.Log(currentCheckPoint);
            GenerateNextLevel(currentCheckPoint);
        }
    }

    public void AddScore(int checkPoint)
    {
        score.text = checkPoint.ToString();
    }
}
