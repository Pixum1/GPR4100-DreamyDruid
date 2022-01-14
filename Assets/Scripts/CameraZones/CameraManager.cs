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

    private Camera cam; //the main camera
    private float cameraWidth;

    private PlayerController player;

    private void Awake() {
        zones = FindObjectsOfType<CameraZone>(); //get all zones
        cam = Camera.main;
        player = Object.FindObjectOfType<PlayerController>();
    }

    private void Start() {
        currentZone = zones[0];
        previousZone = currentZone;
    }

    private void Update() {
        GetCurrentZone();

        if (currentZone != previousZone) {
            SetCameraPosition(); //set camera position if the current zone has been switched (saves performance)
        }

        if (CheckCameraBounds(currentZone) && Time.timeScale > 0) {
            cam.transform.position = new Vector3(player.transform.position.x, currentZone.transform.position.y, cam.transform.position.z);
            AdjustCamEdge(currentZone);
        }
    }

    private bool CheckCameraBounds(CameraZone _zone) {
        return _zone.col.bounds.Contains(GetCameraBounds(_zone)[0]) &&
               _zone.col.bounds.Contains(GetCameraBounds(_zone)[1]) &&
               _zone.col.bounds.Contains(GetCameraBounds(_zone)[2]) &&
               _zone.col.bounds.Contains(GetCameraBounds(_zone)[3]);
    }

    private Vector2[] GetCameraBounds(CameraZone _zone) {
        Vector3 camPos = cam.transform.position;

        float camHeight = (2f * cam.orthographicSize);
        float camWidth = (camHeight * cam.aspect);
        Vector2[] camCorners = new Vector2[4];

        Vector2 leftUpperCorner = new Vector2(camPos.x - camWidth / 2, camPos.y + camHeight / 2);
        Vector2 leftLowerCorner = new Vector2(camPos.x - camWidth / 2, camPos.y - camHeight / 2);
        Vector2 rightUpperCorner = new Vector2(camPos.x + camWidth / 2, camPos.y + camHeight / 2);
        Vector2 rightLowerCorner = new Vector2(camPos.x + camWidth / 2, camPos.y - camHeight / 2);

        camCorners[0] = leftUpperCorner;
        camCorners[1] = leftLowerCorner;
        camCorners[2] = rightUpperCorner;
        camCorners[3] = rightLowerCorner;

        return camCorners;
    }

    private void AdjustCameraToZoneEdge(CameraZone _zone, string _side) {
        if (_side == "left") {
            Vector3 newPos = _zone.col.bounds.min + new Vector3(((2f * cam.orthographicSize) * cam.aspect)/2, 0, 0); //center of the box on the left most edge + half the camera's width
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, cam.transform.position.z);
        }
        if (_side == "right") {
            Vector3 newPos =  _zone.col.bounds.max - new Vector3(((2f*cam.orthographicSize) * cam.aspect)/2, 0, 0); //center of the box on the right most edge - half of the camera's width
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, cam.transform.position.z);
        }
        else {
            Debug.LogWarning("WRONG SIDE NAME!!!");
        }
    }
    private void AdjustCamEdge(CameraZone _zone) {

        //Left Check
        if(!_zone.col.bounds.Contains(GetCameraBounds(_zone)[0]) && !_zone.col.bounds.Contains(GetCameraBounds(_zone)[1])) {
            //move to the right
            AdjustCameraToZoneEdge(_zone, "left");
            return;
        }
        //right Check
        else if(!_zone.col.bounds.Contains(GetCameraBounds(_zone)[2]) && !_zone.col.bounds.Contains(GetCameraBounds(_zone)[3])) {
            //move to the left
            AdjustCameraToZoneEdge(_zone, "right");
            return;
        }
    }

    /// <summary>
    /// Smoothly transition the cameras position to the current zone and adjust its size accordingly.
    /// </summary>
    private void SetCameraPosition() {
        Debug.Log("Setting Camera Position");
        Time.timeScale = 0f;

        Vector3 zonePos = new Vector3(currentZone.transform.position.x, currentZone.transform.position.y, cam.transform.position.z);
        
        cam.orthographicSize = currentZone.transform.localScale.y / 2f; //adjust cam size
        cam.transform.position = Vector3.Lerp(cam.transform.position, zonePos, camSwitchSpeed * 0.02f); //lerp camera to new position

        if(Vector3.Distance(cam.transform.position, zonePos) < .05f) {
            previousZone = currentZone;
            Time.timeScale = 1f;
        }

        cameraWidth = cam.orthographicSize * Screen.width / Screen.height;
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
