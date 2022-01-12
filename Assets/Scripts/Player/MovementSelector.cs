using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovementSelector : MonoBehaviour
{
    [SerializeField]
    List<Behaviour> behaviourScripts;

    [SerializeField]
    int i;

    void Awake()
    {
        //behaviourScripts.Add(GetComponent<PlayerController>());
        behaviourScripts.Add(GetComponent<Grappling>());
        behaviourScripts.Add(GetComponent<Gliding>());
        behaviourScripts.Add(GetComponent<Rolling>());        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchScript();
        }
    }

    void SwitchScript()
    {
        if (i < behaviourScripts.Count-1)
        {
            i++;
        }
        else
        {
            i = 0;
        }

        foreach (Behaviour behaviour in behaviourScripts)
        {
            behaviour.enabled = false;
        }
        
        behaviourScripts[i].enabled = true;
    }    
}