using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Image deathFadeImg;

    [SerializeField]
    private float transitionSpeed;

    private CameraManager cameraManager;

    public Action e_PlayerDied;

    private void Awake() {
        cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update() {
        if (e_PlayerDied == null)
            Debug.LogWarning("Null Reference");

        if (Input.GetButton("Reset"))
            PlayerDies();
    }

    private void OnTriggerStay2D(Collider2D _other) {
        if (_other.CompareTag("Obstacle")) {
            PlayerDies();
        }
    }

    private void PlayerDies() {
        e_PlayerDied?.Invoke();
        cameraManager.ResetCameraPos();
    }
}
