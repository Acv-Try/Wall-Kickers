using UnityEngine;
using UnityEngine.Tilemaps;

public partial class GridGenerator
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap background;
    [SerializeField] private RuleTile GrassRuleTile;

    private void GenerateBackground(LevelData level)
    {
        for(int i = 0; i < level.rows; i++)
        {
            for (int j = 0; j < level.columns; j++)
            {
                if (level.board[i].column[j].type == LevelData.WallType.G)
                {
                    var pos = new Vector3Int(j, level.rows - 1 - i, 0);
                    background.SetTile(origin + pos, GrassRuleTile);
                }
            }
        }
        origin += new Vector3Int(0, level.rows, 0);
        //Debug.Log(origin);
    }
}
