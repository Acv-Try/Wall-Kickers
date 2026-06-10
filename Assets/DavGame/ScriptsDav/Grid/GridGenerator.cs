using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

public partial class GridGenerator : MonoBehaviour
{
    private static GridGenerator instance;
    public static GridGenerator Instance => instance;

    [Header("Tilemaps")]
    [SerializeField] public Tilemap walls;

    [Header("Rule Tiles")]
    [SerializeField] private RuleTile NormalWallRuleTile;

    [Header("Lists")]
    [SerializeField] private List<LevelData> levelsDatas;
    [SerializeField] private List<Level> levels = new();

    private Dictionary<Vector3Int, Wall> wallsScriptsPair;

    private Vector3Int origin;
    private LevelData.Side lastLevelSide = LevelData.Side.Left;

    private void Awake()
    {
        wallsScriptsPair =  new();
        Application.targetFrameRate = 60;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GenerateNextLevel(0);
        GenerateNextLevel(10);
        GenerateNextLevel(20);
        GenerateNextLevel(30);
        GenerateNextLevel(40);
    }

    private void GenerateNextLevel(int num)
    {
        LevelData nextLevel = GetRandomLevel(num, lastLevelSide);
        Level level = new Level(nextLevel.rows, nextLevel.columns, origin);
        Vector3Int lastCellRow = Vector3Int.zero;

        for (int i = nextLevel.rows - 1; i >= 0; i--)
        {
            for (int j = nextLevel.columns - 1; j >= 0; j--)
            {
                Vector3Int pos = new Vector3Int(j, nextLevel.rows - 1 - i, 0);
                RuleTile ruleTile = GetRuleTileFromWallType(nextLevel.board[i].column[j].type);

                var cell = new CellObject(pos + origin, nextLevel.board[i].column[j].type);
                level.CellObjects[i, j] = cell;

                Vector3Int cellPos = pos + origin;

                walls.SetTile(cellPos, ruleTile);

                wallsScriptsPair[cellPos] = new DefaultWall();

                lastCellRow = pos;
            }
        }
        lastLevelSide = nextLevel.endSide;
        //Debug.Log($"Generated level with start side: {nextLevel.startSide} and end side: {nextLevel.endSide}");
        origin += lastCellRow + Vector3Int.up;
        levels.Add(level);
        GenerateBackground(level);
    }

    private void OnDrawGizmos()
    {
        if (levels == null || levels.Count == 0) return;
        Gizmos.color = Color.red;

        foreach (var level in levels)
        {
            for (var i = 0; i < level.CellObjects.GetLength(0); i++)
            {
                for (var j = 0; j < level.CellObjects.GetLength(1); j++)
                {
                    var cell = level.CellObjects[i, j];
                    Gizmos.DrawLine(cell.CellPosition, cell.CellPosition + new Vector3(1, 0, 0));
                    Gizmos.DrawLine(cell.CellPosition, cell.CellPosition + new Vector3(0, 1, 0));

                    // Text Label
                    GUIStyle textStyle = new GUIStyle();
                    textStyle.normal.textColor = Color.white;
                    textStyle.alignment = TextAnchor.MiddleCenter;

                    Vector3 cellCenter = cell.CellPosition + new Vector3(0.5f, 0.5f, 0);
                    Handles.Label(cellCenter, $"({cell.CellPosition.y},{cell.CellPosition.x})", textStyle);
                }
                if (i == level.CellObjects.GetLength(0) - 1)
                {
                    var height = level.CellObjects.GetLength(0);
                    var width = level.CellObjects.GetLength(1);
                    Gizmos.DrawLine(new Vector3(width, 0, 0) + origin, new Vector3(width, height, 0) + origin);
                    Gizmos.DrawLine(new Vector3(0, height, 0) + origin, new Vector3(width, height, 0) + origin);
                }
            }
        }
    }

    private RuleTile GetRuleTileFromWallType(LevelData.WallType type)
    {
        switch (type)
        {
            case LevelData.WallType.N:
                return NormalWallRuleTile;
            default:
                return null;
        }
    }

    private LevelData GetRandomLevel(int cp, LevelData.Side endSide)
    {
        if (cp < 10)
            return levelsDatas[0];

        int group = (cp - 11) / 40;

        int start = group * 5 + 1;
        int end = start + 5;

        start = Mathf.Clamp(start, 1, levelsDatas.Count - 1);
        end = Mathf.Clamp(end, start + 1, levelsDatas.Count);

        int index = 0;

        List<LevelData> rangeLevels = levelsDatas.GetRange(start, end - start);
        rangeLevels.Shuffle();

        foreach (LevelData levelData in rangeLevels)
        {
            if (levelData.startSide == endSide)
            {
                index = levelsDatas.IndexOf(levelData);
                break;
            }
        }
        return levelsDatas[index];
    }

    public void RemoveLevel()
    {
        BoundsInt bounds = new BoundsInt(
            levels[0].Origin.x,
            levels[0].Origin.y,
            levels[0].Origin.z,
            levels[0].CellObjects.GetLength(1),
            levels[0].CellObjects.GetLength(0),
            1
        );

        foreach (var pos in bounds.allPositionsWithin)
        {
            walls.SetTile(pos, null);
        }
        levels.RemoveAt(0);
    }

    public void CheckIfPlayerAboveOfMiddleLevel(Vector3 playerPosition, int currentCheckPoint)
    {
        foreach (var level in levels)
        {
            if (playerPosition.y > level.Origin.y + level.CellObjects.GetLength(0) / 2f)
            {
                RemoveLevel();
                GenerateNextLevel(currentCheckPoint);
                break;
            }
        }
    }

    public Wall GetWallScriptObjectFromWorldPosition(Vector3 worldPosition)
    {
        Vector3Int cell = walls.WorldToCell(worldPosition);

        return wallsScriptsPair.TryGetValue(cell, out var wall)
            ? wall
            : null;
    }
}
