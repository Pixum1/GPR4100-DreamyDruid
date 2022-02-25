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

    [SerializeField]
    private Transform objectToFollow;

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
    private float sizeThreshold;


    private Rect rect;
    [SerializeField]
    private float rectWidth = 5f;
    [SerializeField]
    private float rectHeight = 5f; 
    private float distRight;
    private float distLeft;
    private float distTop;
    private float distBot;

    private void Awake() {
        zones = FindObjectsOfType<CameraZone>(); //get all zones
        cam = Camera.main;

        GetCurrentZone();
        //currentZone = zones[0];
    }

    private void Start() {
        GetCurrentZone();
        
        rect = new Rect(0, 0, rectWidth, rectHeight);
    }
    private void Update() {
        GetCurrentZone();
        //if (previousZone == null) {
        //    ResetCameraPos();
        //}

        if (currentZone != previousZone) {
            SetCameraPosition(); //set camera position if the current zone has been switched (saves performance)
        }

        if (!rect.Contains(objectToFollow.position) && Time.timeScale > 0f) {
            //cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
            CameraMoveByRect();
            AdjustCamEdge(currentZone);
        }
        RecalculateBounds();
    }
    void CameraMoveByRect() {
        RecalculateBounds();

        rect.size = new Vector2(rectWidth, rectHeight);

        
        //left CAM bound within left ZONE bound
        if (GetCameraBounds()[0].x > GetZoneBounds(currentZone)[0]) {
            // PLAYER left of RECT
            if (objectToFollow.position.x < rect.xMin) {
                cam.transform.position -= new Vector3(distLeft, 0, 0); //Move cam left
            }
        }

        // right CAM bound within right ZONE bound
        if (GetCameraBounds()[2].x < GetZoneBounds(currentZone)[1]) {
            // PLAYER right of RECT
            if (objectToFollow.position.x > rect.xMax) {
                cam.transform.position -= new Vector3(distRight, 0, 0);//Move cam right
            }
        }

        // bottom CAM bound within bottom ZONE bound
        if (GetCameraBounds()[3].y > GetZoneBounds(currentZone)[3]) {
            // PLAYER below RECT
            if (objectToFollow.position.y < rect.yMin) {
                cam.transform.position -= new Vector3(0, distBot, 0);// Move cam down
            }
        }

        // top CAM bound within top ZONE bound
        if (GetCameraBounds()[0].y < GetZoneBounds(currentZone)[2]) {
            // PLAYER over RECT
            if (objectToFollow.position.y > rect.yMax) {
                cam.transform.position -= new Vector3(0, distTop, 0); // Move cam up
            }
        }
    }

    private void AdjustCamSize() {
        //1.776f == 16:9 aspect ratio
        if(currentZone.col.bounds.size.x/currentZone.col.bounds.size.y < 1.776f) {
            sizeThreshold = currentZone.transform.localScale.y / 2f * (currentZone.col.bounds.size.x / currentZone.col.bounds.size.y) / 1.776f - .025f;
        }
        else {
            sizeThreshold = currentZone.transform.localScale.y / 2f - .025f;
        }

        float tempSize = currentZone.cameraOrthographicSize;
        if (tempSize > sizeThreshold) {
            tempSize = sizeThreshold;
        }
        //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, tempSize, camSwitchSpeed/3 * 0.02f); //adjust cam size
        cam.orthographicSize = tempSize; //adjust cam size
    }

    private void RecalculateBounds() {
        rect.center = cam.transform.position;

        distRight = rect.xMax - objectToFollow.position.x;//-
        distLeft = rect.xMin - objectToFollow.position.x;//+

        distTop = rect.yMax - objectToFollow.position.y;//-
        distBot = rect.yMin - objectToFollow.position.y;//+
    }

    /// <summary>
    /// Returns an array of Vector2 coordinates containing the dimensions of the Cameras bounds (0=top left, 1=bottom left, 2=top right, 3=bottom right)
    /// </summary>
    /// <returns>Array => (0=top left, 1=bottom left, 2=top right, 3=bottom right)</returns>
    private Vector2[] GetCameraBounds() {
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
    /// Returns an array of float values containing the coordinates of the zone bounds
    /// </summary>
    /// <param name="_zone"></param>
    /// <returns>Array => (0=left Border, 1=right Border, 2=upper Border, 3=lower Border)</returns>
    private float[] GetZoneBounds(CameraZone _zone) {
        Vector3 currentZonePos = _zone.transform.position;

        float[] zoneBorders = new float[4];

        float leftBorder = currentZonePos.x - currentZone.col.bounds.extents.x;
        float rightBorder = currentZonePos.x + currentZone.col.bounds.extents.x;
        float upperBorder = currentZonePos.y + currentZone.col.bounds.extents.y;
        float lowerBorder = currentZonePos.y - currentZone.col.bounds.extents.y;

        zoneBorders[0] = leftBorder;
        zoneBorders[1] = rightBorder;
        zoneBorders[2] = upperBorder;
        zoneBorders[3] = lowerBorder;

        return zoneBorders;
    }

    /// <summary>
    /// Adjusts the camera's position based on it's location inside the given "CameraZone"
    /// </summary>
    /// <param name="_zone"></param>
    private void AdjustCamEdge(CameraZone _zone) {
        //Left edge Check
        if(!_zone.col.bounds.Contains(GetCameraBounds()[0]) && !_zone.col.bounds.Contains(GetCameraBounds()[1])) {
            Vector3 newPos = _zone.col.bounds.min + new Vector3(cameraWidth / 2, 0, 0); //center of the box on the left most edge + half the camera's width
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, cam.transform.position.z);
            //return;
        }//right edge Check
        if (!_zone.col.bounds.Contains(GetCameraBounds()[2]) && !_zone.col.bounds.Contains(GetCameraBounds()[3])) {
            Vector3 newPos = _zone.col.bounds.max - new Vector3(cameraWidth / 2, 0, 0); //center of the box on the right most edge - half of the camera's width
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, cam.transform.position.z);
            //return;
        }
        //Bottom check
        if (!_zone.col.bounds.Contains(GetCameraBounds()[1]) && !_zone.col.bounds.Contains(GetCameraBounds()[3])) {
            Vector3 newPos = _zone.col.bounds.min + new Vector3(0, cameraHeight / 2, 0);
            cam.transform.position = new Vector3(cam.transform.position.x, newPos.y, cam.transform.position.z);
        }
        //Top Check
        if (!_zone.col.bounds.Contains(GetCameraBounds()[0]) && !_zone.col.bounds.Contains(GetCameraBounds()[2])) {
            Vector3 newPos = _zone.col.bounds.max - new Vector3(0, cameraHeight / 2, 0);
            cam.transform.position = new Vector3(cam.transform.position.x, newPos.y, cam.transform.position.z);
        }
    }

    /// <summary>
    /// Smoothly transition the cameras position to the current zone and adjust its size accordingly.
    /// </summary>
    private void SetCameraPosition() {
        Time.timeScale = 0f;
        AdjustCamSize();

        cam.transform.position = Vector3.MoveTowards(cam.transform.position, CalculateNewPosition(), camSwitchSpeed * Time.fixedUnscaledDeltaTime);

        if (Vector3.Distance(cam.transform.position, CalculateNewPosition()) < 0.25f) {
            previousZone = currentZone;
            Time.timeScale = 1f;
            AdjustCamEdge(currentZone);
        }
    }

    private Vector3 CalculateNewPosition() {
        Vector3 newPos;
        Vector3 sideX, sideY;

        float distanceToBottom = objectToFollow.position.y - cameraHeight / 2;
        float distanceToTop = objectToFollow.position.y + cameraHeight / 2;

        sideX = Vector3.zero;
        sideY = new Vector3(0, objectToFollow.position.y, 0);

        if(currentZone != null) {
            //if the player moves into the zone from its RIGHT side
            if (objectToFollow.position.x > currentZone.col.bounds.center.x) {
                sideX = currentZone.col.bounds.max - new Vector3(cameraWidth / 2, 0, 0);
            }

            //if the player moves into the zone from its LEFT side
            else if (objectToFollow.position.x < currentZone.col.bounds.center.x) {
                sideX = currentZone.col.bounds.min + new Vector3(cameraWidth / 2, 0, 0);
            }

            //if the player moves into the LOWER PART OF THE ZONE
            if (distanceToBottom < currentZone.col.bounds.min.y) {
                sideY = currentZone.col.bounds.min + new Vector3(0, cameraHeight / 2, 0);
            }

            //if the player moves into the UPPER PART OF THE ZONE
            else if (distanceToTop > currentZone.col.bounds.max.y) {
                sideY = currentZone.col.bounds.max - new Vector3(0, cameraHeight / 2, 0);
            }
        }
        

        newPos = new Vector3(sideX.x, sideY.y, cam.transform.position.z);
        return newPos;
    }

    public void ResetCameraPos() {
        currentZone = null;
        previousZone = null;
        GetCurrentZone();
        cam.transform.position = new Vector3(objectToFollow.position.x, objectToFollow.position.y, cam.transform.position.z);
        AdjustCamEdge(currentZone);
        //cam.transform.position = CalculateNewPosition();
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
        Transform camera = FindObjectOfType<Camera>().transform;
        Gizmos.DrawWireCube(camera.position, new Vector2(rectWidth, rectHeight));

    }
}
