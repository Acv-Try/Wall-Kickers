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

    public int topMarkerRow = -1;
    public int topMarkerColumn = -1;
    public int bottomMarkerRow = -1;
    public int bottomMarkerColumn = -1;

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

                if (type == WallType.E ||
                    type == WallType.G)
                {
                    if (firstCellColumn == -1)
                        firstCellColumn = col;

                    lastCellColumn = col;
                }
            }
        }
    }
    public void CalculateMarkers()
    {
        Debug.Log("---------enter");
        topMarkerRow = -1;
        topMarkerColumn = -1;
        bottomMarkerRow = -1;
        bottomMarkerColumn = -1;
        Debug.Log(
            $"top = [{topMarkerRow} ; {topMarkerColumn}] " +
            $"bottom  = [{topMarkerRow} ; {topMarkerColumn}] ");

        //highest G cell = top connector
        for (int i = rows - 1; i >= 0 && topMarkerRow == -1; i--)
        {
            for (int j = 0; j < columns; j++)
            {
                if (board[i].column[j].type == WallType.G)
                {
                    topMarkerRow = i + 1;
                    topMarkerColumn = j;
                    Debug.Log($"top G = [{topMarkerRow} ; {topMarkerColumn}]");
                    break;
                }
            }
        }

        //lowest G cell = bottom connector
        for (int i = 0; i < rows && bottomMarkerRow == -1; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                if (board[i].column[j].type == WallType.G)
                {
                    bottomMarkerRow = i;
                    bottomMarkerColumn = j;
                    Debug.Log($"bottom G = [{bottomMarkerRow} ; {bottomMarkerColumn}]");
                    break;
                }
            }
        }
    }
}
