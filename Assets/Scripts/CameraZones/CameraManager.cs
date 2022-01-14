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

    private PlayerController player;

    private float cameraHeight {
        get {
            return (2f * cam.orthographicSize);
        }
    }
    private float cameraWidth {
        get {
            return (cameraHeight * cam.aspect);
        }
    }
    public float size;


    private Rect rect;
    [SerializeField]
    private float rectWidth = 5f;
    [SerializeField]
    private float rectHeight = 5f; 
    private float distRight;
    private float distLeft;
    private float distTop;
    private float distBot;
    private Vector2 botLeftPoint;
    private Vector2 botRightPoint;
    private Vector2 topLeftPoint;
    private Vector2 topRightPoint;

    private void Awake() {
        zones = FindObjectsOfType<CameraZone>(); //get all zones
        cam = Camera.main;
        player = Object.FindObjectOfType<PlayerController>();
    }

    private void Start() {
        GetCurrentZone();
        
        rect = new Rect(0, 0, rectWidth, rectHeight);
    }
    private void Update() {
        GetCurrentZone();

        if (currentZone != previousZone || previousZone == null) {
            SetCameraPosition(); //set camera position if the current zone has been switched (saves performance)
        }

        if (cameraWidth < currentZone.col.bounds.size.x) {
            if (CheckCameraBounds(currentZone)) {
                ///INSERT IF STATEMENT --> if player is outside of deadzone
                if (!rect.Contains(player.transform.position)) {
                    //cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
                    CameraMoveByRect();
                    AdjustCamEdge(currentZone);
                }
            }
        }
        RecalculateBounds();
    }
    void CameraMoveByRect() {
        RecalculateBounds();

        rect.size = new Vector2(rectWidth, rectHeight);

        //Move cam left
        if (GetCameraBounds(currentZone)[0].x > currentZone.transform.position.x - currentZone.col.bounds.extents.x)// left CAM bound within left ZONE bound
        {
            if (player.transform.position.x < rect.xMin)// PLAYER left of RECT
            {
                cam.transform.position -= new Vector3(distLeft, 0, 0);
            }
        }

        //Move cam right
        if (GetCameraBounds(currentZone)[2].x < currentZone.transform.position.x + currentZone.col.bounds.extents.x)// right CAM bound within right ZONE bound
        {
            if (player.transform.position.x > rect.xMax)// PLAYER right of RECT
            {
                cam.transform.position -= new Vector3(distRight, 0, 0);
            }
        }

        // Move cam down
        if (GetCameraBounds(currentZone)[3].y > currentZone.transform.position.y - currentZone.col.bounds.extents.y)// bottom CAM bound within bottom ZONE bound
        {
            if (player.transform.position.y < rect.yMin)// PLAYER below RECT
            {
                cam.transform.position -= new Vector3(0, distBot, 0);
            }
        }

        // Move cam up
        if (GetCameraBounds(currentZone)[0].y < currentZone.transform.position.y + currentZone.col.bounds.extents.y)// top CAM bound within top ZONE bound
    {
            if (player.transform.position.y > rect.yMax)// PLAYER over RECT
            {
                cam.transform.position -= new Vector3(0, distTop, 0);
            }
        }
    }

    private void RecalculateBounds() {
        botLeftPoint = new Vector2(rect.xMin, rect.yMin);
        botRightPoint = new Vector2(rect.xMax, rect.yMin);
        topLeftPoint = new Vector2(rect.xMin, rect.yMax);
        topRightPoint = new Vector2(rect.xMax, rect.yMax);

        rect.center = cam.transform.position;

        distRight = rect.xMax - player.transform.position.x;//-
        distLeft = rect.xMin - player.transform.position.x;//+

        distTop = rect.yMax - player.transform.position.y;//-
        distBot = rect.yMin - player.transform.position.y;//+
    }

    /// <summary>
    /// Returns true if every corner of the camera is within a given "CameraZone"
    /// </summary>
    /// <param name="_zone"></param>
    /// <returns></returns>
    private bool CheckCameraBounds(CameraZone _zone) {
        return _zone.col.bounds.Contains(GetCameraBounds(_zone)[0]) &&
               _zone.col.bounds.Contains(GetCameraBounds(_zone)[1]) &&
               _zone.col.bounds.Contains(GetCameraBounds(_zone)[2]) &&
               _zone.col.bounds.Contains(GetCameraBounds(_zone)[3]);
    }

    /// <summary>
    /// Returns an array of Vector2 coordinates containing the dimensions of the Cameras bounds (0=top left, 1=bottom left, 2=top right, 3=bottom right)
    /// </summary>
    /// <returns>Array => (0=top left, 1=bottom left, 2=top right, 3=bottom right)</returns>
    private Vector2[] GetCameraBounds(CameraZone _zone) {
        Vector3 camPos = cam.transform.position;

        Vector2[] camCorners = new Vector2[4];

        Vector2 leftUpperCorner = new Vector2(camPos.x - cameraWidth / 2, camPos.y + cameraHeight / 2);
        Vector2 leftLowerCorner = new Vector2(camPos.x - cameraWidth / 2, camPos.y - cameraHeight / 2);
        Vector2 rightUpperCorner = new Vector2(camPos.x + cameraWidth / 2, camPos.y + cameraHeight / 2);
        Vector2 rightLowerCorner = new Vector2(camPos.x + cameraWidth / 2, camPos.y - cameraHeight / 2);

        camCorners[0] = leftUpperCorner;
        camCorners[1] = leftLowerCorner;
        camCorners[2] = rightUpperCorner;
        camCorners[3] = rightLowerCorner;
        return camCorners;
    }

    /// <summary>
    /// Adjusts the camera's position based on it's location inside the given "CameraZone"
    /// </summary>
    /// <param name="_zone"></param>
    private void AdjustCamEdge(CameraZone _zone) {
        Debug.Log("AdjustingCam");
        //Left edge Check
        if(!_zone.col.bounds.Contains(GetCameraBounds(_zone)[0]) && !_zone.col.bounds.Contains(GetCameraBounds(_zone)[1])) {
            Vector3 newPos = _zone.col.bounds.min + new Vector3(cameraWidth / 2, 0, 0); //center of the box on the left most edge + half the camera's width
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, cam.transform.position.z);
            //return;
        }//right edge Check
        if (!_zone.col.bounds.Contains(GetCameraBounds(_zone)[2]) && !_zone.col.bounds.Contains(GetCameraBounds(_zone)[3])) {
            Vector3 newPos = _zone.col.bounds.max - new Vector3(cameraWidth / 2, 0, 0); //center of the box on the right most edge - half of the camera's width
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, cam.transform.position.z);
            //return;
        }
        //Bottom check
        if (!_zone.col.bounds.Contains(GetCameraBounds(_zone)[1]) && !_zone.col.bounds.Contains(GetCameraBounds(_zone)[3])) {
            Vector3 newPos = _zone.col.bounds.min + new Vector3(0, cameraHeight / 2, 0);
            cam.transform.position = new Vector3(cam.transform.position.x, newPos.y, cam.transform.position.z);
        }
        //Top Check
        if (!_zone.col.bounds.Contains(GetCameraBounds(_zone)[0]) && !_zone.col.bounds.Contains(GetCameraBounds(_zone)[2])) {
            Vector3 newPos = _zone.col.bounds.max - new Vector3(0, cameraHeight / 2, 0);
            cam.transform.position = new Vector3(cam.transform.position.x, newPos.y, cam.transform.position.z);
        }
    }

    /// <summary>
    /// Smoothly transition the cameras position to the current zone and adjust its size accordingly.
    /// </summary>
    private void SetCameraPosition() {
        Debug.Log("Setting Camera Position");
        Time.timeScale = 0f;

        Vector3 newPos;

        //cam.orthographicSize = currentZone.transform.localScale.y / 2f; //adjust cam size
        cam.orthographicSize = size; //adjust cam size

        //if camera is smaller than the CameraZone
        if (cameraWidth < currentZone.col.bounds.size.x) {

            //if player is on the right side of the CameraZone
            if(player.transform.position.x > currentZone.col.bounds.max.x) {
                Vector3 side = currentZone.col.bounds.min + new Vector3(cameraWidth / 2, 0, 0);

                newPos = new Vector3(side.x, currentZone.transform.position.y, cam.transform.position.z);
            }
            //if player is on the left side of the CameraZone
            else {
                Vector3 side = currentZone.col.bounds.max - new Vector3(cameraWidth / 2, 0, 0);

                newPos = new Vector3(side.x, currentZone.transform.position.y, cam.transform.position.z);
            }
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, newPos, camSwitchSpeed * 0.02f);

            if(CheckCameraBounds(currentZone)) {
                previousZone = currentZone;
                Time.timeScale = 1f;
                AdjustCamEdge(currentZone);
            }
        }
        else {

            newPos = new Vector3(currentZone.transform.position.x, currentZone.transform.position.y, cam.transform.position.z);

            cam.transform.position = Vector3.MoveTowards(cam.transform.position, newPos, camSwitchSpeed * 0.02f); //lerp camera to new position

            if (CheckCameraBounds(currentZone)) {
                previousZone = currentZone;
                Time.timeScale = 1f;
            }
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cam.transform.position, new Vector2(rectWidth, rectHeight));
    }
}
