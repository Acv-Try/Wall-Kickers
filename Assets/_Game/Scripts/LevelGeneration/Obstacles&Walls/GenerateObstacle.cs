using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateObstacle : MonoBehaviour
{
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
    public void SaveAsPrefab()
    {
        string originalPath = savePath;
        originalPath += prefabName + i.ToString() + prefabFormat;
        PrefabUtility.SaveAsPrefabAsset(generatedRoot, originalPath).AddComponent<BoxCollider2D>();
        i++;
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