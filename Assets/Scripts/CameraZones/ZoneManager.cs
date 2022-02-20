using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField]
    public LayerMask playerLayer;
    /// <summary>
    /// Creates a new CameraZone and returns it.
    /// </summary>
    /// <returns>Instance of CameraZone</returns>
    public GameObject CreateZone() {
        GameObject zone = new GameObject("CameraZone");
        zone.transform.SetParent(transform);
        zone.transform.localScale = new Vector3(40, 22.5f, 1);
        zone.AddComponent<CameraZone>().playerLayer = playerLayer;
        zone.AddComponent<BoxCollider>().isTrigger = true;
        return zone;
    }
}
