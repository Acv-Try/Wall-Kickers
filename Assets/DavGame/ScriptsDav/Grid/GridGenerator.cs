using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GridGenerator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI score;
    private static GridGenerator instance;
    public static GridGenerator Instance => instance;

    [Header("Rule Tiles")]
    [SerializeField] private RuleTile NormalWallRuleTile;

    [Header("Lists")]
    [SerializeField] private List<Level> levels;
    private List<Level> levelsGenerated = new();

    [SerializeField] private GameObject grid;
    [SerializeField] private int heigthOffset;
    private Vector3Int heigthCount;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        score.text = "0";
        background.ClearAllTiles();

        for (int i = 0; i < levelsGenerated.Count; i++)
        {
            Destroy(levelsGenerated[i].gameObject);
        }
        levelsGenerated.Clear();

        heigthCount = Vector3Int.zero;

        GenerateNextLevel(0);
        GenerateNextLevel(20);
    }

    public void GenerateNextLevel(int checkPoint)
    {
        int index = 0;
        if (checkPoint < 10) index = Random.Range(0, 2);
        else if (checkPoint >= 10 && checkPoint < 50) index = Random.Range(2, 6);
        else if (checkPoint >= 50 && checkPoint < 90) index = Random.Range(6, 10);
        else if (checkPoint >= 90 && checkPoint < 130) index = Random.Range(10, 14);
        else if (checkPoint >= 130 && checkPoint < 170) index = Random.Range(14, 18);
        else if (checkPoint >= 170 && checkPoint < 210) index = Random.Range(18, 22);
        else index = Random.Range(3, 22);

        index = Mathf.Clamp(index, 0, levels.Count - 1);

        var level = levels[index];

        (int minH, int maxH) edges = GenerateBackground(level);

        heigthCount = new Vector3Int(heigthCount.x, heigthCount.y - level.MinHeightY, 0);

        var levelInstance = Instantiate(level, heigthCount, Quaternion.identity, grid.transform);

        levelInstance.MaxHeightY = edges.Item2;
        levelInstance.MinHeightY = edges.Item1;
        levelInstance.Origin = heigthCount;

        levelsGenerated.Add(levelInstance);
        heigthCount += new Vector3Int(0, levelInstance.MaxHeightY - levelInstance.MinHeightY, 0);

    }

    public void RemoveLevel()
    {
        var level = levelsGenerated[0];
        levelsGenerated.RemoveAt(0);
        BoundsInt bounds = new BoundsInt(
            level.Origin.x,
            level.Origin.y,
            level.Origin.z,
            level.levelData.columns,
            level.MaxHeightY,
            1
        );
        Debug.Log(bounds.x + " "  + bounds.y+ " " + bounds.z);
        foreach (var pos in bounds.allPositionsWithin)
        {
            background.SetTile(pos, null);
        }

        Destroy(level.gameObject);
    }

    public void CheckIfPlayerAboveOfMiddleLevel(Vector3 playerPosition, int currentCheckPoint)
    {
        AddScore(currentCheckPoint);
        if (currentCheckPoint < 10) return;
        currentLevel = levelsGenerated[1];
        Debug.Log(currentLevel.Origin + " " + currentCheckPoint);
        float middleY = currentLevel.Origin.y +
                (currentLevel.MaxHeightY - currentLevel.MinHeightY) / 2f;

        if (playerPosition.y > middleY)
        {
            Debug.Log("Pos" + playerPosition.y + " Mid " + middleY);
            RemoveLevel();
            GenerateNextLevel(currentCheckPoint);
        }
    }

    public void AddScore(int checkPoint)
    {
        score.text = checkPoint.ToString();
    }
}
