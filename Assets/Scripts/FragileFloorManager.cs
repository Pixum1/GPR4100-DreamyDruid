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
            f.StopAllCoroutines();
            f.transform.position = f.startPos;
            f.SetState(true);
        }
    }
}
