using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TutorialText))]
public class TutorialTextEditor : Editor
{
    int selected = 0;

    [SerializeField]
    private string keyword;
    [SerializeField]
    private int index;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(20f);
        GUILayout.Label("Displayed Tutorial Text");
        TutorialText text = (TutorialText)target;

        GUILayout.BeginVertical();

        text.tutorialText = EditorGUILayout.TextArea(text.tutorialText);

        GUILayout.Space(20f);
        GUILayout.Label("Create new Keywords");

        keyword = EditorGUILayout.TextField("Keyword: ", keyword);
        index = EditorGUILayout.IntField("Sprite Asset Index: ", index);
        if (GUILayout.Button("Add Keyword"))
        {
            if (keyword != null && !text.keywordIndex.Contains(index))
            {
                text.keywords.Add(keyword);
                text.keywordIndex.Add(index);
            }
        }

        GUILayout.EndVertical();

        GUILayout.Space(20f);
        GUILayout.Label("Preview Settings");

        string[] options = new string[]
        {
            "Controller", "Keyboard",
        };
        selected = EditorGUILayout.Popup("Preview Input Methods", selected, options);

        if (selected == 0)
        {
            text.inputMethod = "Controller";
            text.currentSpriteAsset = text.controllerSprites;
        }
        else if (selected == 1)
        {
            text.inputMethod = "Keyboard";
            text.currentSpriteAsset = text.keyboardSprites;
        }

        if (GUILayout.Button("Show Text"))
        {
            text.UpdateText();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}