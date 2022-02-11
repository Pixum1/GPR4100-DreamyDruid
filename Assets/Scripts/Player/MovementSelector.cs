using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MovementSelector : MonoBehaviour
{
    Behaviour currentScript;
    [SerializeField]
    Image[] selectIcons; //0 = Owl, 1 = Frog, 2 = Armadillo, 3 = Human
    [SerializeField]
    private ParticleSystem evolveParticles;
    private bool isBear;

    private void Start() {
        if(currentScript != null) {
            if (GetComponent<Gliding>().isActiveAndEnabled) {
                ActivateSelectionIcon(selectIcons[0]);
            }
            else if (GetComponent<Grappling>().isActiveAndEnabled) {
                ActivateSelectionIcon(selectIcons[1]);
            }
            else if (GetComponent<Rolling>().isActiveAndEnabled) {
                ActivateSelectionIcon(selectIcons[2]);
            }
            else {

                ActivateSelectionIcon(selectIcons[3]);
            }
        }
        else {
            ActivateSelectionIcon(selectIcons[3]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Evolve Vertical") == 1f) {
            SwitchScript(GetComponent<Gliding>());
            ActivateSelectionIcon(selectIcons[0]);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxisRaw("Evolve Horizontal") == -1f) {
            SwitchScript(GetComponent<Grappling>());
            ActivateSelectionIcon(selectIcons[1]);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw("Evolve Vertical") == -1f) {
            SwitchScript(GetComponent<Rolling>());
            ActivateSelectionIcon(selectIcons[2]);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxisRaw("Evolve Horizontal") == 1f) {
            SwitchToHuman();
            ActivateSelectionIcon(selectIcons[3]);
        }
    }

    private void ActivateSelectionIcon(Image _selectIcon) {
        for (int i = 0; i < selectIcons.Length; i++) {
            selectIcons[i].enabled = false;
        }
        _selectIcon.enabled = true;
    }

    void SwitchScript(Behaviour _script)
    {
        if(currentScript != null)
            currentScript.enabled = false;

        if (currentScript != _script)
            evolveParticles.Play();

        isBear = false;
        _script.enabled = true;
        currentScript = _script;

    }    
    private void SwitchToHuman() {
        if(currentScript != null)
            currentScript.enabled = false;

        if (!isBear)
            evolveParticles.Play();

        currentScript = null;
        isBear = true;
    }
}