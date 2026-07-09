using UnityEngine;
using UnityEngine.Tilemaps;

public partial class GridGenerator
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap background;
    [SerializeField] private RuleTile GrassRuleTile;

    private (Vector3Int, Vector3Int) GenerateBackground(Level level)
    {
        bool isFirst = true;
        Vector3Int MinHeight = Vector3Int.zero;
        Vector3Int MaxHeight = Vector3Int.zero;
        for (int i = 0; i < level.levelData.rows; i++)
        {
            for (int j = 0; j < level.levelData.columns; j++)
            {
                if (level.levelData.board[i].column[j].type == LevelData.WallType.G)
                {
                    if (isFirst)
                    {
                        MinHeight = new Vector3Int(j, i, 0);
                        isFirst = false;
                    }
                    var pos = new Vector3Int(j - MinHeight.x, i - MinHeight.y, 0);
                    background.SetTile(heightCount + pos, GrassRuleTile);


                    if (MaxHeight.y < i)
                    {
                        MaxHeight = new Vector3Int(j, i, 0);
                    }
                }
            }
        }
        return (MinHeight, MaxHeight);
    }
}
