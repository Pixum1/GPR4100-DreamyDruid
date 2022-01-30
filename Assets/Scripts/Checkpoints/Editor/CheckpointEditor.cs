using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckpointManager))]
public class CheckpointEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        CheckpointManager manager = (CheckpointManager)target;

        if(GUILayout.Button("Create Checkpoint")) {
            GameObject checkpoint = manager.CreateCheckpoint();
            Selection.activeGameObject = checkpoint;
        }
    }
}
