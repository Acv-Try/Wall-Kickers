using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateObstacle : MonoBehaviour
{
    [SerializeField] private WallType type;
    [SerializeField] private ObstaclePiece bottomPrefab;
    [SerializeField] private ObstaclePiece topPrefab;
    [SerializeField] private ObstaclePiece[] bodyPrefabs;
    [SerializeField] private ObstaclePiece[] defaultBodyPrefabs;
    [SerializeField] private int logSize;
    [SerializeField] private bool withBranch = true;
    private GameObject generatedRoot;

    public GameObject GeneratedRoot => generatedRoot;
    public WallType Type => type;

    public void Generate()
    {
        Clear();

        generatedRoot = new GameObject("GeneratedObstacle");

        ObstaclePiece current =
            Instantiate(bottomPrefab, Vector3.zero, Quaternion.identity, generatedRoot.transform);
        for (int i = 0; i < logSize; i++)
        {
            ObstaclePiece next = null;
            if (withBranch)
            {
                next = Instantiate(
                    bodyPrefabs[Random.Range(0, bodyPrefabs.Length)],
                    generatedRoot.transform);
            }
            else
            {
                next = Instantiate(
                    defaultBodyPrefabs[Random.Range(0, defaultBodyPrefabs.Length)],
                    generatedRoot.transform);
            }

            Attach(current, next);
            current = next;
        }

        var top = Instantiate(topPrefab, generatedRoot.transform);
        Attach(current, top);
    }

    private void Attach(ObstaclePiece previous, ObstaclePiece next)
    {
        Vector3 offset = previous.TopAttach.position - next.BottomAttach.position;
        next.transform.position += offset;
    }

    public void Clear()
    {
        if (generatedRoot != null)
        {
            DestroyImmediate(generatedRoot);
            generatedRoot = null;
        }
    }
}























//using NUnit.Framework;
//using System;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;
//using Random = UnityEngine.Random;

//public class GenerateObstacle : MonoBehaviour
//{

//    [SerializeField] private WallType Type;
//    [SerializeField] private ObstaclePiece bottomPrefab;
//    [SerializeField] private ObstaclePiece topPrefab;
//    [SerializeField] private ObstaclePiece[] bodyPrefabs;

//    [SerializeField] private int logSize;
//    [SerializeField] private string savePath;
//    [SerializeField] private string prefabName;

//    private GameObject generatedRoot;
//    string prefabFormat = ".prefab";
//    float topPoint, half;
//    int i = 0;

//    public void Generate()
//    {
//        Clear();

//        generatedRoot = new GameObject("GeneratedObstacle");

//        ObstaclePiece current =
//            Instantiate(bottomPrefab, Vector3.zero,
//                Quaternion.identity,
//                generatedRoot.transform);

//        for (int i = 0; i < logSize; i++)
//        {
//            ObstaclePiece next =
//                Instantiate(
//                    bodyPrefabs[Random.Range(0, bodyPrefabs.Length)],
//                    generatedRoot.transform);

//            Attach(current, next);

//            current = next;
//        }

//        ObstaclePiece top =
//            Instantiate(topPrefab, generatedRoot.transform);

//        Attach(current, top);
//    }
//    private void Attach(
//        ObstaclePiece previous,
//        ObstaclePiece next)
//    {
//        Vector3 offset = previous.TopAttach.position
//            - next.BottomAttach.position;

//        next.transform.position += offset;
//    }
//    private void Clear()
//    {
//        if (generatedRoot != null)
//        {
//            DestroyImmediate(generatedRoot);
//            generatedRoot = null;
//        }
//    }
//public void SaveAsPrefab()
//{
//    string path = savePath + prefabName + i + prefabFormat;

//    generatedRoot.AddComponent<BoxCollider2D>();
//    generatedRoot.AddComponent(GetWallType(Type));
//    SetColliderSize(generatedRoot);
//    PrefabUtility.SaveAsPrefabAsset(generatedRoot, path);

//    i++;
//}
//private Type GetWallType(WallType type)
//{
//    return type switch
//    {
//        WallType.Default => typeof(DefaultWall),
//        WallType.Bounce => typeof(BounceWall),
//        _ => throw new ArgumentOutOfRangeException(nameof(type))
//    };
//}
//private void SetColliderSize(GameObject prefab)
//{
//    Bounds bounds = new Bounds();

//    SpriteRenderer[] renderers = prefab.GetComponentsInChildren<SpriteRenderer>();

//    if (renderers.Length > 0)
//    {
//        bounds = renderers[0].bounds;
//        for (int i = 1; i < renderers.Length; i++)
//        {
//            if (renderers[i].tag == "Branch") continue;
//            bounds.Encapsulate(renderers[i].bounds);
//        }

//        Vector2 size = bounds.size;
//        Vector2 center = bounds.center;
//        BoxCollider2D collider = prefab.GetComponent<BoxCollider2D>();
//        collider.size = size;
//        collider.offset = prefab.transform.InverseTransformPoint(center);
//    }
//}
//}
//[CustomEditor(typeof(GenerateObstacle))]
//public class PNGToTileMapEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        if (target == null) return;

//        DrawDefaultInspector();
//        GenerateObstacle generate = (GenerateObstacle)target;
//        if (generate == null) return;

//        if (GUILayout.Button("Generate Obstacle"))
//        {
//            generate.Generate();
//        }
//        if (GUILayout.Button("Save Prefab"))
//        {
//            generate.SaveAsPrefab();
//        }

//    }
//}