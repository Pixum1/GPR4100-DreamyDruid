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

    private Color newColor;

    public bool canReset = true;

    private bool isFadingIn;
    private bool isFadingOut;

    private void Awake() {
        cameraManager = FindObjectOfType<CameraManager>();
        newColor = new Color(0, 0, 0, 0);
    }

    private void Start()
    {
        canReset = true;
    }

    private void Update() {
        if (e_PlayerDied == null)
            Debug.LogWarning("Null Reference");

        if (Input.GetButtonDown("Reset") && !isFadingIn && !isFadingOut && canReset) {
            PlayerDies();
        }

        if (isFadingIn) {
            FadeIn();
        }
        if (isFadingOut) {
            FadeOut();
        }
    }

    private void OnTriggerStay2D(Collider2D _other) {
        if (_other.CompareTag("Obstacle") && (!isFadingIn && !isFadingOut)) {
            PlayerDies();
        }
    }

    private void PlayerDies() {

        isFadingIn = true;
    }

    private void FadeIn() {
        Time.timeScale = 0f;
        if (newColor.a < 1) {
            newColor.a += Time.unscaledDeltaTime * transitionSpeed;
            deathFadeImg.color = newColor;
        }
        else {
            isFadingIn = false;
            isFadingOut = true;
            e_PlayerDied?.Invoke();
        }
    }
    private void FadeOut() {
        cameraManager.ResetCameraPos();
        if (newColor.a > 0) {
            newColor.a -= Time.unscaledDeltaTime * transitionSpeed;
            deathFadeImg.color = newColor;
        }
        else {
            isFadingOut = false;
            Time.timeScale = 1;
        }
    }
}
