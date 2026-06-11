using UnityEngine;
using UnityEngine.Tilemaps;

public partial class GridGenerator
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap background;
    [SerializeField] private RuleTile GrassRuleTile;
    int columns;
    int rows;

    //private void GenerateBackground(Level level)
    //{
    //    columns = level.CellObjects.GetLength(1) - 1;
    //    rows = level.CellObjects.GetLength(0) - 1;
    //    for (int i = rows; i >= 0; i--)
    //    {
    //        for (int j = columns; j >= 0; j--)
    //        {
    //            if (level.CellObjects[i, j].CellType != LevelData.WallType.E)
    //            {
    //               background.SetTile(level.CellObjects[i, j].CellPosition, GrassRuleTile);
    //            }
    //        }
    //    }
    //}
}
