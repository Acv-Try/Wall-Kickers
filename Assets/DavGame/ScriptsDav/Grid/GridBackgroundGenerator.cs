using UnityEngine;
using UnityEngine.Tilemaps;

public partial class GridGenerator
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap background;
    [SerializeField] private RuleTile GrassRuleTile;

    private void GenerateBackground(Level level)
    {
        for (int i = level.CellObjects.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = level.CellObjects.GetLength(1) - 1; j >= 0; j--)
            {
                if (level.CellObjects[i, j].CellType == LevelData.WallType.N)
                {
                    Vector3Int pos1 = level.CellObjects[i, j-1].CellPosition;
                    Vector3Int pos2 = level.CellObjects[i, j].CellPosition;
                    Vector3Int pos3 = level.CellObjects[i, j+1].CellPosition;
                    background.SetTile(pos1, GrassRuleTile);
                    background.SetTile(pos2, GrassRuleTile);
                    background.SetTile(pos3, GrassRuleTile);
                }
            }
        }
    }
}
