using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "InsideGrass", menuName = "2D/Tiles/RuleTiles/Inside Grass")]
public class InsideGrass : RuleTile<InsideGrass.Neighbor>
{
    public List<TileBase> innerTiles;
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int InnerGrass = 3;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.InnerGrass: return innerTiles.Contains(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }
}
