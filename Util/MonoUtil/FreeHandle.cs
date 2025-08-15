// OnSceneGUI
// private void DrawNPC(LevelData data)
// {
//     EditorGUI.BeginChangeCheck();
//     var pos    = data.NPCPosition.Position;
//     var oldPos = (Vector3)GridManager.GridToPos(pos);
//     var style = new GUIStyle
//     {
//         normal =
//         {
//             textColor = Color.yellow
//         }
//     };
//
//     Handles.color = Color.black;
//     Handles.Label(
//         oldPos + (Vector3.down * 0.4f) +
//         (Vector3.right * 0.4f), "死神", style);
//
//     var newPos = Handles.FreeMoveHandle(oldPos, .45f, new Vector3(.25f, .25f, .25f),
//         Handles.RectangleHandleCap);
//
//     var newGrid = GridManager.PosToGrid(newPos);
//
//     if (!EditorGUI.EndChangeCheck()) return;
//     Undo.RecordObject(target, "Free Move Handle");
//
//     data.NPCPosition.Position = newGrid;
//     EditorUtility.SetDirty(data);
//     AssetDatabase.SaveAssets();
// }