using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class Nightmare : MonoBehaviour
{
    private PlayerController playerController;

    Transform player;

    [SerializeField]
    float maxTime;

    Checkpoint currentCheckpoint;
    CheckpointManager cPManager;

    [SerializeField]
    List<Vector3Int> emptyNightmareTiles = new List<Vector3Int>();
    [SerializeField]
    List<Vector3Int> usedNightmareTiles = new List<Vector3Int>();
    [SerializeField]
    Vector3Int startPos;
    [SerializeField]
    Vector3Int resetPos;

    [SerializeField]
    int i;

    [SerializeField]
    Tilemap nightmareTilemap;

    [SerializeField]
    TileBase nmTile;

    TileInfoTool tIT;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        tIT = GetComponent<TileInfoTool>();
        cPManager = GameObject.Find("CheckPointManager").GetComponent<CheckpointManager>();
        playerController.pHealth.e_PlayerDied += new Action(ResetNightmare);
        emptyNightmareTiles = tIT.LoadTileData().surroundingTilesData;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        usedNightmareTiles.Add(startPos);
        emptyNightmareTiles.Remove(startPos);
        GetNextTile();
        
    }

    void GetNextTile()
    {
        StartCoroutine(GetSurroundingTiles(usedNightmareTiles[i]));
        if (i < usedNightmareTiles.Count - 1)
        {
            i++;
        }
    }

    private IEnumerator GetSurroundingTiles(Vector3Int _tile)
    {
        List<Vector3Int> bufferTiles = new List<Vector3Int>();
        float dist = Vector3.Distance(_tile, player.position);
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                Vector3Int offset = new Vector3Int(x, y, 0);
                Vector3Int pos = _tile + offset;

                if (emptyNightmareTiles.Contains(pos)&&dist<50)
                {
                    bufferTiles.Add(pos);
                    emptyNightmareTiles.Remove(pos);
                }
            }
        }
        float f = Mathf.Max(0.00075f * Mathf.Pow(dist, 2.8f), 1);
        float timeToNext = maxTime / f;
        yield return new WaitForSeconds(timeToNext);
        usedNightmareTiles.AddRange(bufferTiles);
        
        nightmareTilemap.SetTile(_tile,nmTile);
        GetNextTile();
    }

    private void ResetNightmare()
    {
        StopAllCoroutines();
        nightmareTilemap.ClearAllTiles();
        usedNightmareTiles.Clear();
        emptyNightmareTiles.Clear();
        emptyNightmareTiles = tIT.LoadTileData().surroundingTilesData;
        currentCheckpoint = cPManager.currentCP;
        resetPos = currentCheckpoint.nightmarePosWhenCollected;
        usedNightmareTiles.Add(resetPos);
        emptyNightmareTiles.Remove(resetPos);
        i = 0;
        GetNextTile();
    }

    private void OnDisable()
    {
        playerController.pHealth.e_PlayerDied -= ResetNightmare;
    }
}
