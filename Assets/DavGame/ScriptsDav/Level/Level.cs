using UnityEngine;
using System.Collections.Generic;

public class Level 
{
    private CellObject[,] cellObjects;
    private Vector3Int origin;

    public Vector3Int Origin { get => origin; private set => origin = value; }
    public CellObject[,] CellObjects => cellObjects;

    public Level(int rows, int columns, Vector3Int origin)
    {
        cellObjects = new CellObject[rows, columns];
        this.origin = origin;
    }
}
