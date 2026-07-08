using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
[System.Serializable]
public class LevelData : ScriptableObject
{
    public enum WallType
    {
        E,
        G,
    }

    [System.Serializable]
    public class Cell
    {
        public WallType type;
        public int rotation;
        public int wallHeight;
    }

    [System.Serializable]
    public class Row
    {
        public Cell[] column;

        public Row(int size)
        {
            column = new Cell[size];

            for (int i = 0; i < size; i++)
            {
                column[i] = new Cell
                {
                    type = WallType.E,
                    rotation = 0,
                    wallHeight = 0
                };
            }
        }

        public void ClearRow()
        {
            for (int i = 0; i < column.Length; i++)
            {
                column[i].type = WallType.E;
                column[i].rotation = 0;
            }
        }
    }

    public int firstCellColumn = -1;
    public int lastCellColumn = -1;
    
    public int markerRow = -1;
    public int markerColumn = -1;

    //public int bottomMarkerRow = -1;
    //public int bottomMarkerColumn = -1;
    //public Vector2Int bottomMarker = new Vector2Int(-1,-1);
    //public Vector2Int topMarker = new Vector2Int(-1,-1);
    public int columns;
    public int rows;
    public Row[] board;

    public void Clear()
    {
        if (board == null) return;

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] != null)
            {
                board[i].ClearRow();
            }
        }
    }

    public void CreateNewBoard()
    {
        if (columns <= 0 || rows <= 0) return;

        board = new Row[rows];

        for (int i = 0; i < rows; i++)
        {
            board[i] = new Row(columns);
        }
        Clear();
    }

    public void CalculateSides()
    {
        firstCellColumn = -1;
        lastCellColumn = -1;
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = columns - 1; col >= 0; col--)
            {
                var type = board[row].column[col].type;

                if (type != LevelData.WallType.E &&
                    type != LevelData.WallType.G)
                {
                    if (firstCellColumn == -1)
                        firstCellColumn = col;

                    lastCellColumn = col;
                }
            }
        }
    }
    public void CalculateMarker()
    {
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < columns; j++)
            {
                var type = board[i].column[j].type;
                if(type == WallType.G)
                {
                    markerRow = i + 1;
                    markerColumn = j;
                    return;
                }
            }
        }
    }
}
