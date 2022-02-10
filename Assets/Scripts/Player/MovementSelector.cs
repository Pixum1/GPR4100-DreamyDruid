using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MovementSelector : MonoBehaviour
{
    Behaviour currentScript;
    [SerializeField]
    Image[] scriptIcons; //0 = Owl, 1 = Frog, 2 = Armadillo, 3 = Human
    Image currentScriptIcon;

    private void Awake() {
        currentScriptIcon = scriptIcons[3]; //Human
    }
    private void Update()
    {
        foreach(Image icon in scriptIcons) {
            if (icon != currentScriptIcon) { 
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, .5f);
            }
            else {
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1f);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw("Evolve Horizontal") == 1f) {
            SwitchToHuman();
            currentScriptIcon = scriptIcons[3]; //Human
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxisRaw("Evolve Vertical") == -1f) {
            SwitchScript(GetComponent<Rolling>());
            currentScriptIcon = scriptIcons[2]; //Armadillo
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Evolve Horizontal") == -1f) {
            SwitchScript(GetComponent<Grappling>());
            currentScriptIcon = scriptIcons[1]; //Frog
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetAxisRaw("Evolve Vertical") == 1f) {
            SwitchScript(GetComponent<Gliding>());
            currentScriptIcon = scriptIcons[0]; //Owl
        }
    }

    void SwitchScript(Behaviour _script)
    {
        if(currentScript != null)
            currentScript.enabled = false;

        _script.enabled = true;
        currentScript = _script;
    }    
    private void SwitchToHuman() {
        if(currentScript != null)
            currentScript.enabled = false;
    }
}