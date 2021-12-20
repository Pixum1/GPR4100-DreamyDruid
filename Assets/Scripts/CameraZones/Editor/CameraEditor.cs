using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZoneManager))]
public class CameraEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        ZoneManager zone = (ZoneManager)target;

        if(GUILayout.Button("Create new zone")) {
            CameraZone newZone = zone.CreateZone();
            Selection.activeGameObject = newZone.gameObject; //Select the new Zone in hierarchy
        }
    }
}
