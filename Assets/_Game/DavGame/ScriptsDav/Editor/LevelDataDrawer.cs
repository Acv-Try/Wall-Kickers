using UnityEditor;
using UnityEngine;
using static LevelData;

[CustomEditor(typeof(LevelData))]
[CanEditMultipleObjects]
public class LevelDataDrawer : Editor
{
    private LevelData Data => (LevelData)target;

    private static LevelData.WallType selectedWallType;
    private static int selectedWallRotation;
    private static int selectedWallHeight;

    private GUIStyle cellStyle;
    private bool isPainting;

    public override void OnInspectorGUI()
    {
        if (cellStyle == null)
        {
            cellStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.black }
            };
        }

        serializedObject.Update();

        DrawSizeFields();

        EditorGUILayout.Space();

        ClearBoard();

        EditorGUILayout.Space();

        if (IsBoardValid())
        {
            DrawBoard();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSizeFields()
    {
        SerializedProperty rowsProp = serializedObject.FindProperty("rows");
        SerializedProperty columnsProp = serializedObject.FindProperty("columns");

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(rowsProp);
        EditorGUILayout.PropertyField(columnsProp);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            Undo.RecordObject(Data, "Resize Grid");

            Data.CreateNewBoard();

            EditorUtility.SetDirty(Data);
        }

        EditorGUILayout.Space();

        selectedWallType =
            (LevelData.WallType)EditorGUILayout.EnumPopup(
                "Wall Type",
                selectedWallType);

        selectedWallRotation =
            EditorGUILayout.IntPopup(
                "Rotation",
                selectedWallRotation,
                new string[] { "0", "180" },
                new int[] { 0, 180 });

        selectedWallHeight =
            EditorGUILayout.IntPopup(
                "Wall Height",
                selectedWallHeight,
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7" },
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
    }

    private void DrawBoard()
    {
        const float size = 25f;
        Event e = Event.current;
        for (int row = 0; row < Data.rows; row++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int col = 0; col < Data.columns; col++)
            {
                var cell = Data.board[Data.rows - 1 - row].column[col];

                GUI.color = GetColor(cell.type);

                Rect rect = GUILayoutUtility.GetRect(size, size);
                GUI.Box(rect, "");
                if (rect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown)
                    {
                        isPainting = true;
                        if (e.button == 0) 
                            SetRectOnGridInspector(cell);
                        else if (e.button == 1) 
                            EraseRectOnGridInspector(cell);
                    }
                    else if (e.type == EventType.MouseDrag && isPainting)
                    {
                        if (e.button == 0)
                            SetRectOnGridInspector(cell);
                        else if (e.button == 1)
                            EraseRectOnGridInspector(cell);
                    }
                }

                GUI.color = Color.white;

                GUI.Label(rect, cell.type.ToString(), cellStyle);
            }

            EditorGUILayout.EndHorizontal();
        }
        if (e.type == EventType.MouseUp) isPainting = false;
    }

    public void SetRectOnGridInspector(Cell cell)
    {
        Undo.RecordObject(Data, "Paint Wall");

        cell.type = selectedWallType;
        cell.rotation = selectedWallRotation;
        cell.wallHeight = selectedWallHeight;

        Data.CalculateSides();

        EditorUtility.SetDirty(Data);
    }

    public void EraseRectOnGridInspector(Cell cell)
    {
        Undo.RecordObject(Data, "Erase Wall");
        cell.type = LevelData.WallType.E;
        cell.rotation = 0;
        cell.wallHeight = 0;
        Data.CalculateSides();
        EditorUtility.SetDirty(Data);
    }

    private Color GetColor(LevelData.WallType type)
    {
        switch (type)
        {
            case LevelData.WallType.E:
                return Color.white;

            case LevelData.WallType.N:
                return new Color(0.6f, 0.3f, 0.1f);

            case LevelData.WallType.G:
                return new Color(0.2f, 0.8f, 0.2f);

            default:
                return Color.gray;
        }
    }

    private void ClearBoard()
    {
        if (GUILayout.Button("Clear Board"))
        {
            Undo.RecordObject(Data, "Clear Board");

            Data.Clear();

            EditorUtility.SetDirty(Data);
        }
    }

    private bool IsBoardValid()
    {
        return Data.board != null &&
               Data.board.Length == Data.rows &&
               Data.columns > 0 &&
               Data.rows > 0;
    }

    
}