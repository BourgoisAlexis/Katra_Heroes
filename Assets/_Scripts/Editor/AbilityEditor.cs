using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ability), true)]
public class AbilityEditor : Editor
{
    #region Variables
    Ability instance;
    SerializedObject getTarget;

    protected List<string> propertiesToHide = new List<string>();
    #endregion


    private void OnEnable()
    {
        instance = (Ability)target;
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((Ability)target), typeof(Ability), false);
        GUI.enabled = true;


        propertiesToHide.Add("m_Script");


        if (instance.Targetting == e_targetting.None)
        {
            propertiesToHide.Add("RangeType");
            propertiesToHide.Add("Range");
            propertiesToHide.Add("Exceptions");
            propertiesToHide.Add("GetOccupied");
        }
        else if (instance.Targetting == e_targetting.AutomaticTarget)
        {
            propertiesToHide.Add("Range");
            propertiesToHide.Add("Exceptions");
            propertiesToHide.Add("GetOccupied");
        }

        DrawPropertiesExcluding(serializedObject, propertiesToHide.ToArray());
        serializedObject.ApplyModifiedProperties();
        
        propertiesToHide.Clear();
    }
}