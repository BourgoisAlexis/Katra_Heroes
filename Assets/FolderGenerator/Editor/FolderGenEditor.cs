using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FolderGenerator))]
public class FolderGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FolderGenerator gen = (FolderGenerator)target;
        
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
            gen.GenerateFolders();

        if (GUILayout.Button("Rename"))
            gen.RenameFolders();

        if (GUILayout.Button("Get"))
            gen.GetFolders();
    }
}
