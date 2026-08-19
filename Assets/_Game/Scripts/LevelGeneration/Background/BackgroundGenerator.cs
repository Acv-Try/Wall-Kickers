using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] private RuleTile grassRuleTile;
    private Tilemap background;

    private void Awake()
    {
        background = GetComponent<Tilemap>();
    }

    public void Paint(Level level, Vector3Int origin)
    {
        for (int i = 0; i < level.levelData.rows; i++)
        {
            for (int j = 0; j < level.levelData.columns; j++)
            {
                if (level.levelData.board[i].column[j].type != LevelData.WallType.G)
                    continue;

                Vector3Int localPos = new Vector3Int(j, i, 0);

                background.SetTile(origin + localPos, grassRuleTile);
            }
        }
    }

    public void Remove(Level level)
    {
        BoundsInt bounds = new BoundsInt(
            level.Origin.x,
            level.Origin.y,
            level.Origin.z,
            level.levelData.columns,
            level.MaxHeight.y,
            1
        );

        foreach (var pos in bounds.allPositionsWithin)
            background.SetTile(pos, null);
    }

    public void Reset()
    {
        background.ClearAllTiles();
    }

    public Vector3 CellToWorld(Vector3Int cellPos) => background.CellToWorld(cellPos);
}
//bool isFirst = true;

//LevelInfo info = new LevelInfo();
//info.Origin = origin;
//Vector3Int min = Vector3Int.zero;
//Vector3Int max = Vector3Int.zero;

//for (int i = 0; i < level.levelData.rows; i++)
//{
//    for (int j = 0; j < level.levelData.columns; j++)
//    {
//        if (level.levelData.board[i].column[j].type != LevelData.WallType.G) continue;

//        if (isFirst)
//        {
//            min = new Vector3Int(j, i, 0);
//            isFirst = false;
//        }

//        var pos = new Vector3Int(j - min.x, i - min.y, 0);
//        background.SetTile(info.Origin + pos, grassRuleTile);

//        if (max.y < i)
//            max = new Vector3Int(j, i, 0);
//    }
//}
//info.Min = min;
//info.Max = max;
//info.NextOrigin = origin += new Vector3Int(max.x - min.x, max.y - min.y + 1);
//return info;

//private BackgroundInfo GenerateBackground(Level level, Vector3Int origin)
//{
//    Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, 0);
//    Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, 0);

//    // 1. First pass = compute true bounds
//    for (int i = 0; i < level.levelData.rows; i++)
//    {
//        for (int j = 0; j < level.levelData.columns; j++)
//        {
//            if (level.levelData.board[i].column[j].type != LevelData.WallType.G)
//                continue;

//            min.x = Mathf.Min(min.x, j);
//            min.y = Mathf.Min(min.y, i);

//            max.x = Mathf.Max(max.x, j);
//            max.y = Mathf.Max(max.y, i);
//        }
//    }

//    // 2. Now we know exact shape size
//    Vector3Int size = new Vector3Int(
//        max.x - min.x,
//        max.y - min.y,
//        0
//    );

//    // 3. Second pass = place tiles
//    for (int i = 0; i < level.levelData.rows; i++)
//    {
//        for (int j = 0; j < level.levelData.columns; j++)
//        {
//            if (level.levelData.board[i].column[j].type != LevelData.WallType.G)
//                continue;

//            Vector3Int localPos = new Vector3Int(
//                j - min.x,
//                i - min.y,
//            0
//            );

//            background.SetTile(origin + localPos, GrassRuleTile);
//        }
//    }

//    // 4. Build return info
//    BackgroundInfo info = new BackgroundInfo
//    {
//        Origin = origin,
//        Min = min,
//        Max = max,
//        NextOrigin = origin + new Vector3Int(size.x + 1, size.y + 1, 0)
//    };

//    return info;
//}