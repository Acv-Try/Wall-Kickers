using UnityEngine;

public class CellObject
{
    private Vector3Int cellPosition;
    private LevelData.WallType cellType;

    public LevelData.WallType CellType { get => cellType; private set => cellType = value; }
    public Vector3Int CellPosition { get => cellPosition; set => cellPosition = value; }

    public CellObject(Vector3Int cellPosition, LevelData.WallType cellType)
    {
        CellPosition = cellPosition;
        CellType = cellType;
    }
}
