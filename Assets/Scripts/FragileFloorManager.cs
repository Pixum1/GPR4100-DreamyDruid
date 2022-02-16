using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileFloorManager : MonoBehaviour
{
    private PlayerHealth player;
    private List<FragileFloor> allFloorTiles = new List<FragileFloor>();
    [SerializeField]
    private GameObject floorTile;

    private void Awake()
    {
        player = FindObjectOfType<PlayerHealth>();
    }

    private void Start()
    {
        allFloorTiles.AddRange(FindObjectsOfType<FragileFloor>());

        player.e_PlayerDied += Reset;
    }


    private void Reset()
    {
        foreach(FragileFloor f in allFloorTiles)
        {
            GameObject g = Instantiate(floorTile);
            g.transform.position = f.transform.position;
            Destroy(f.gameObject);
        }
        allFloorTiles.Clear();
        allFloorTiles.AddRange(FindObjectsOfType<FragileFloor>());
    }
}
