using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap background;
    [SerializeField] private RuleTile grassRuleTile;

    #region
    private static BackgroundGenerator _instance;
    public static BackgroundGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<BackgroundGenerator>();
                if (_instance != null)
                {
                    Debug.LogWarning($"BackgroundGenerator is not found in the scene!");
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
    }
    #endregion

    public (Vector3Int min, Vector3Int max) Generate(Level level, Vector3Int heightCount)
    {
        bool isFirst = true;
        Vector3Int min = Vector3Int.zero;
        Vector3Int max = Vector3Int.zero;

        for (int i = 0; i < level.levelData.rows; i++)
        {
            for (int j = 0; j < level.levelData.columns; j++)
            {
                if (level.levelData.board[i].column[j].type != LevelData.WallType.G) continue;

                if (isFirst)
                {
                    min = new Vector3Int(j, i, 0);
                    isFirst = false;
                }

                var pos = new Vector3Int(j - min.x, i - min.y, 0);
                background.SetTile(heightCount + pos, grassRuleTile);

                if (max.y < i)
                    max = new Vector3Int(j, i, 0);
            }
        }

        return (min, max);
    }

    public void Clear(Level level)
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

    public void ClearAll()
    {
        background.ClearAllTiles();
    }
}
