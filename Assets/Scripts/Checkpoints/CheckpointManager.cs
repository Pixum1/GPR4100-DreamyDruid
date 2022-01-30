using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Checkpoint currentCP;
    private Checkpoint[] checkpoints;

    [SerializeField]
    private PlayerController player;
    private void Awake() {
        player = FindObjectOfType<PlayerController>();
    }
    private void Start() {
        checkpoints = FindObjectsOfType<Checkpoint>();
        player.pHealth.e_PlayerDied += new Action(Respawn);
    }

    private void Update() {
        foreach(Checkpoint cp in checkpoints) {
            if (cp.isActive) {
                currentCP = cp;
            }
        }
    }
    public GameObject CreateCheckpoint() {
        GameObject newCP = new GameObject("Checkpoint");
        newCP.transform.SetParent(transform);
        newCP.AddComponent<Checkpoint>();
        newCP.AddComponent<BoxCollider2D>().isTrigger = true;
        return newCP;
    }

    private void Respawn() {
        if (currentCP == null) {
            Debug.Log("No checkpoint collected");
            return;
        }

        player.transform.position = currentCP.playerPosWhenCollected;
        FindObjectOfType<CameraManager>().ResetCameraPos();
    }
    private void OnDisable() {
        player.pHealth.e_PlayerDied -= Respawn;
    }
}
