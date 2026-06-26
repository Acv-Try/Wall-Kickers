using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateObstacle : MonoBehaviour
{
    private enum ObstacleType
    {
        Default,
        Bounce
    }
    [SerializeField] private ObstacleType obstacleType;
    [SerializeField] private ObstaclePiece bottomPrefab;
    [SerializeField] private ObstaclePiece topPrefab;
    [SerializeField] private ObstaclePiece[] bodyPrefabs;

    [SerializeField] private int logSize;
    [SerializeField] private string savePath;
    [SerializeField] private string prefabName;

    private GameObject generatedRoot;
    string prefabFormat = ".prefab";
    float topPoint, half;
    int i = 0;
    
    public void Generate()
    {
        Clear();

        generatedRoot = new GameObject("GeneratedObstacle");

        ObstaclePiece current =
            Instantiate(bottomPrefab, Vector3.zero,
                Quaternion.identity,
                generatedRoot.transform);

        for (int i = 0; i < logSize; i++)
        {
            ObstaclePiece next =
                Instantiate(
                    bodyPrefabs[Random.Range(0, bodyPrefabs.Length)],
                    generatedRoot.transform);

            Attach(current, next);

            current = next;
        }

        ObstaclePiece top =
            Instantiate(topPrefab, generatedRoot.transform);

        Attach(current, top);
    }
    private void Attach(
        ObstaclePiece previous,
        ObstaclePiece next)
    {
        Vector3 offset = previous.TopAttach.position
            - next.BottomAttach.position;

        next.transform.position += offset;
    }
    private void Clear()
    {
        if (generatedRoot != null)
        {
            DestroyImmediate(generatedRoot);
        }
    }
    private void SaveAsPrefab()
    {
        string path = savePath + prefabName + i + prefabFormat;

        generatedRoot.AddComponent<BoxCollider2D>();
        generatedRoot.AddComponent(GetWallType(obstacleType));

        PrefabUtility.SaveAsPrefabAsset(generatedRoot, path);

        i++;
    }
    private Type GetWallType(ObstacleType type)
    {
        return type switch
        {
            ObstacleType.Default => typeof(BaseWall),
            ObstacleType.Bounce => typeof(BounceWall),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
    [CustomEditor(typeof(GenerateObstacle))]
    public class PNGToTileMapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GenerateObstacle generate = (GenerateObstacle)target;

            if (GUILayout.Button("Generate Obstacle"))
            {
                generate.Generate();
            }
            if (GUILayout.Button("Save Prefab"))
            {
                generate.SaveAsPrefab();
            }

        }
    }
}