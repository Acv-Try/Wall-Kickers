using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateObstacle))]
public class GenerateObstacleEditor : Editor
{
    private string savePath = "Assets/_Game/Prefabs/Levels/Components/WoodWall/Walls";
    private string prefabName = "Obstacle";
    private int counter;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var generator = (GenerateObstacle)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Obstacle"))
        {
            generator.Generate();
        }

        savePath = EditorGUILayout.TextField("Save Path", savePath);
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);

        if (GUILayout.Button("Save Prefab"))
        {
            SavePrefab(generator);
        }
    }

    private void SavePrefab(GenerateObstacle generator)
    {
        if (generator.GeneratedRoot == null)
            return;

        string path = $"{savePath}{prefabName}_{counter}.prefab";

        var root = generator.GeneratedRoot;

        ObstaclePrefabBuilder.ApplyCollider(root);
        ObstaclePrefabBuilder.ApplyWallScript(root, generator.Type);
        ObstaclePrefabBuilder.ApplyWallTag(root, "Wall");

        PrefabUtility.SaveAsPrefabAsset(root, path);

        counter++;
    }
}