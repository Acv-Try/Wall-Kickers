using System;
using UnityEngine;

public static class ObstaclePrefabBuilder
{
    public static void ApplyCollider(GameObject root)
    {
        if (!root.TryGetComponent<BoxCollider2D>(out var collider))
            collider = root.AddComponent<BoxCollider2D>();

        Bounds bounds = CalculateBounds(root);

        collider.size = bounds.size;
        collider.offset = root.transform.InverseTransformPoint(bounds.center);
    }

    public static void ApplyWallTag(GameObject root, WallType type)
    {
        Type componentType = type switch
        {
            WallType.Default => typeof(DefaultWall),
            WallType.Bounce => typeof(BounceWall),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        if (root.GetComponent(componentType) == null)
            root.AddComponent(componentType);
    }
    
    private static Bounds CalculateBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
            return new Bounds(root.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            if (renderers[i].tag == "Branch") continue;
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}