using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour 
{
    [SerializeField] private Tilemap tilemap;
    public int LastTileY;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        LastTileY = tilemap.cellBounds.yMax;
    }
}
