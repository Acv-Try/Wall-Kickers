using UnityEngine;
using UnityEngine.Tilemaps;

public partial class GridGenerator
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap background;
    [SerializeField] private RuleTile GrassRuleTile;

    private (int, int) GenerateBackground(Level level)
    {
        bool isFirst = true;
        int MinHeightY = 0;
        int MaxHeightY = 0;
        for (int i = 0; i < level.levelData.rows; i++)
        {
            for (int j = 0; j < level.levelData.columns; j++)
            {
                if (level.levelData.board[i].column[j].type == LevelData.WallType.G)
                {
                    if (isFirst)
                    {
                        MinHeightY = i;
                        isFirst = false;
                    }
                    var pos = new Vector3Int(j, level.levelData.rows - 1 - i, 0);
                    background.SetTile(heightCount + pos, GrassRuleTile);
                    MaxHeightY = i;
                }
            }
        }
        return (MinHeightY, MaxHeightY);
    }
}
