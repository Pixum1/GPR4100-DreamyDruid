using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CameraZone currentZone; //the zone the player is currently in
    private CameraZone previousZone; //the zone the player was previously in
    private CameraZone[] zones; //all zones in the scene

    [SerializeField][Tooltip("The speed at which the camera moves over to the new zone")]
    private float camSwitchSpeed;
    [SerializeField]
    private float camFollowSpeed;

    private Camera cam; //the main camera

    private void Awake() {
        zones = FindObjectsOfType<CameraZone>(); //get all zones
        cam = Camera.main;
    }

    private void Start() {
        currentZone = zones[0];
        previousZone = currentZone;
    }

    private void Update() {

        GetCurrentZone();
        float cameraWidth = cam.orthographicSize * Screen.width / Screen.height;

        if (currentZone != previousZone) {
            //set camera position if the current zone has been switched (saves performance)
            SetCameraPosition();
        }
        else if (currentZone == zones[0]) {

            if (Vector3.Distance(cam.transform.position, currentZone.followBorderPosition) > 3) {
                cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(currentZone.followBorderPosition.x, currentZone.followBorderPosition.y, cam.transform.position.z), camFollowSpeed * Time.deltaTime);
            }
        }

    }

    /// <summary>
    /// Smoothly transition the cameras position to the current zone and adjust its size accordingly.
    /// </summary>
    private void SetCameraPosition() {
        Time.timeScale = 0f;

        Vector3 zonePos = new Vector3(currentZone.transform.position.x, currentZone.transform.position.y, cam.transform.position.z);
        
        cam.orthographicSize = currentZone.transform.localScale.y / 2f; //adjust cam size
        cam.transform.position = Vector3.Lerp(cam.transform.position, zonePos, camSwitchSpeed * 0.02f); //lerp camera to new position

        if(Vector3.Distance(cam.transform.position, zonePos) < .05f) {
            previousZone = currentZone;
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Get the zone the player is currently standing in.
    /// </summary>
    private void GetCurrentZone() {
        foreach (CameraZone zone in zones) {
            if (zone.m_isActive) {
                currentZone = zone;
            }
        }
    }
}
