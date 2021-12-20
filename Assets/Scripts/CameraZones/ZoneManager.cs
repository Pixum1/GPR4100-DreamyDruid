using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField]
    private CameraZone zonePrefab;

    /// <summary>
    /// Creates a new CameraZone and returns it.
    /// </summary>
    /// <returns>Instance of CameraZone</returns>
    public CameraZone CreateZone() {
        CameraZone zone = Instantiate(zonePrefab);
        zone.transform.SetParent(transform);
        return zone;
    }
}
