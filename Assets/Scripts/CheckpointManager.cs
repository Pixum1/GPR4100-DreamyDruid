using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Checkpoint currentCP;
    private Checkpoint[] checkpoints;

    [SerializeField]
    private PlayerController player;
    private void Awake() {
        player = FindObjectOfType<PlayerController>();
    }
    private void Start() {
        checkpoints = FindObjectsOfType<Checkpoint>();
        player.e_PlayerDied += new Action(Respawn);
    }

    private void Update() {
        foreach(Checkpoint cp in checkpoints) {
            if (cp.isActive) {
                currentCP = cp;
            }
        }
    }

    private void Respawn() {
        if (currentCP == null)
            return;

        Vector2 newPlayerPos = new Vector2(currentCP.transform.position.x, currentCP.GetComponent<Collider2D>().bounds.min.y + player.transform.localScale.y);
        player.transform.position = newPlayerPos;
        FindObjectOfType<CameraManager>().ResetCameraPos();
    }
}
