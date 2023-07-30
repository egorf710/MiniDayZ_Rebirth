using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JunkGroundGenerator))]
public class JunkGroundGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("GenerateJunk"))
        {
            JunkGroundGenerator newMapLoader = target as JunkGroundGenerator;
            newMapLoader.Generate();
        }
        if (GUILayout.Button("ClearMap"))
        {
            JunkGroundGenerator newMapLoader = target as JunkGroundGenerator;
            newMapLoader.Clear();
        }

        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
