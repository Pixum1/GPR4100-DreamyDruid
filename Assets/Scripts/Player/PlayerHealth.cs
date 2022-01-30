using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    public Action e_PlayerDied;

    private void Update() {
        if (e_PlayerDied == null)
            Debug.LogWarning("Null Reference");

        if (Input.GetKey(KeyCode.R))
            e_PlayerDied?.Invoke();
    }
}
