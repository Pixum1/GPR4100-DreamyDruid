using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZoneManager))]
public class CameraEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        ZoneManager zone = (ZoneManager)target;

        if(GUILayout.Button("Create new zone")) {
            GameObject newZone = zone.CreateZone();
            Selection.activeGameObject = newZone; //Select the new Zone in hierarchy
        }
    }
}
