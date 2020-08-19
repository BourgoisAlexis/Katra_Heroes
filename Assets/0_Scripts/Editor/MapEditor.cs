using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = (MapGenerator)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
            map.GenerateMap();
        
        if (GUILayout.Button("Colorise"))
            map.UpdateColor();

        if (GUILayout.Button("Save"))
            map.SaveMap();
    }
}
