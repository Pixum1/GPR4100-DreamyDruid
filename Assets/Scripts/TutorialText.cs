using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class TutorialText : MonoBehaviour
{
    [Header("TextInput")]
    [SerializeField]
    private TMP_Text textField;

    public const string CONTROLLER_ABILITY_BUTTON = "<sprite=3>";
    public const string KEYBOARD_ABILITY_BUTTON = "<sprite=2>";
    public const string CONTROLLER_JUMP_BUTTON = "<sprite=6>";
    public const string KEYBOARD_JUMP_BUTTON = "<sprite=0>";
    public const string CONTROLLER_RESET_BUTTON = "<sprite=7>";
    public const string KEYBOARD_RESET_BUTTON = "<sprite=8>";
    public const string CONTROLLER_PAUSE_BUTTON = "<sprite=5>";
    public const string KEYBOARD_PAUSE_BUTTON = "<sprite=1>";

    private const string controller = "Controller";
    private const string keyboard = "Keyboard";
    [HideInInspector] public string inputMethod = controller;

    public string tutorialText;

    [SerializeField]
    private GameObject trigger;
    [SerializeField]
    private LayerMask playerLayer;
    private bool triggered = false;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        textField.color = new Color(textField.color.r, textField.color.g, textField.color.b, 0);
    }

    private void Update()
    {
        #region Get Input Method
        if (Input.GetAxisRaw("ControllerInput") != 0 && inputMethod != controller)
        {
            StopCoroutine(FadeText());
            inputMethod = "Controller";
            if (textField.color.a > 0)
                StartCoroutine(FadeText());
        }
        if (Input.GetAxisRaw("KeyboardInput") != 0 && inputMethod != keyboard)
        {
            StopCoroutine(FadeText());
            inputMethod = "Keyboard";
            if(textField.color.a > 0)
                StartCoroutine(FadeText());
        }
        #endregion

        if (Physics2D.OverlapBox(trigger.transform.position, trigger.transform.localScale, 0, playerLayer) && !triggered)
        {
            UpdateText();
            StartCoroutine(TriggerText());
        }
    }

    public void UpdateText()
    {
        string tempText = tutorialText;

        if(inputMethod == controller)
        {
            if (tutorialText.Contains("<ability>"))
            {
                textField.text = tempText.Replace("<ability>", CONTROLLER_ABILITY_BUTTON);
            }
            if (tutorialText.Contains("<jump>"))
            {
                textField.text = tempText.Replace("<jump>", CONTROLLER_JUMP_BUTTON);
            }
            if (tutorialText.Contains("<reset>"))
            {
                textField.text = tempText.Replace("<reset>", CONTROLLER_RESET_BUTTON);
            }
            if (tutorialText.Contains("<pause>"))
            {
                textField.text = tempText.Replace("<pause>", CONTROLLER_PAUSE_BUTTON);
            }
        }
        else if(inputMethod == keyboard)
        {
            if (tutorialText.Contains("<ability>"))
            {
                textField.text = tempText.Replace("<ability>", KEYBOARD_ABILITY_BUTTON);
            }
            if (tutorialText.Contains("<jump>"))
            {
                textField.text = tempText.Replace("<jump>", KEYBOARD_JUMP_BUTTON);
            }
            if (tutorialText.Contains("<reset>"))
            {
                textField.text = tempText.Replace("<reset>", KEYBOARD_RESET_BUTTON);
            }
            if (tutorialText.Contains("<pause>"))
            {

                textField.text = tempText.Replace("<pause>", KEYBOARD_PAUSE_BUTTON);
            }
        }
    }

    private IEnumerator TriggerText()
    {
        triggered = true;
        while(textField.color.a < 1)
        {
            textField.color = new Color(textField.color.r, textField.color.g, textField.color.b, textField.color.a + Time.deltaTime * 2f);
            yield return null;
        }
    }
    private IEnumerator FadeText()
    {
        while (textField.color.a > 0)
        {
            textField.color = new Color(textField.color.r, textField.color.g, textField.color.b, textField.color.a - Time.deltaTime * 4f);
            yield return null;
        }
        UpdateText();
        while (textField.color.a < 1)
        {
            textField.color = new Color(textField.color.r, textField.color.g, textField.color.b, textField.color.a + Time.deltaTime * 4f);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(trigger.transform.position, trigger.transform.localScale);
    }
}

[CustomEditor(typeof(TutorialText))]
public class TutorialTextEditor: Editor
{
    int selected = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(20f);
        GUILayout.Label("Preview Settings");
        TutorialText text = (TutorialText)target;

        string[] options = new string[]
        {
            "Controller", "Keyboard",
        };
        selected = EditorGUILayout.Popup("Preview Input Methods", selected, options);

        if (selected == 0)
            text.inputMethod = "Controller";
        else if (selected == 1)
            text.inputMethod = "Keyboard";

        if (GUILayout.Button("Show Text"))
        {
            text.UpdateText();
        }
    }
}
