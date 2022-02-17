using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Nightmare : MonoBehaviour
{
    private PlayerController playerController;
    Transform player;
    [SerializeField]
    float maxTime;
    [SerializeField]
    int pathPositionsAmount=20;
    [SerializeField]
    float pathFollowDistance = 15;
    Checkpoint currentCheckpoint;
    CheckpointManager cPManager;
    [SerializeField]
    Vector3Int resetPosition;
    [SerializeField]
    Vector3Int startPosition;
    [SerializeField]
    Tilemap nightmareTilemap;
    [SerializeField]
    Tilemap groundTilemap;
    [SerializeField]
    TileBase nmTile;
    List<Vector3> playerPathPoints = new List<Vector3>();
    public bool active;
    [SerializeField]
    Checkpoint startCheckpoint;
    [SerializeField]
    CrowAnimation crow;
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        cPManager = GameObject.Find("CheckPointManager").GetComponent<CheckpointManager>();
        playerController.pHealth.e_PlayerDied += new Action(ResetNightmare);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        active = false;
    }

    private void Update()
    {
        currentCheckpoint = cPManager.currentCP;
        if (!active)
        {
            if (startCheckpoint == currentCheckpoint)
            {
                StartCoroutine(StartNightmare());
            }
        }
    }

    IEnumerator StartNightmare()
    {
        crow.active=true;
        active = true;
        StartCoroutine(GetPathPoints());
        yield return new WaitForSeconds(6);
        StartCoroutine(GetSurroundingTiles(Vector3Int.RoundToInt(playerPathPoints[8])));
    }

    private void ResetNightmare()
    {
        StopAllCoroutines();
        nightmareTilemap.ClearAllTiles();
        currentCheckpoint = cPManager.currentCP;
        Vector3 resetPositonV3 = currentCheckpoint.transform.GetChild(0).transform.position;
        resetPosition = new Vector3Int(Mathf.RoundToInt(resetPositonV3.x), Mathf.RoundToInt(resetPositonV3.y), 0);
        StartCoroutine(GetPathPoints());
        StartCoroutine(GetSurroundingTiles(resetPosition));
    }
    private IEnumerator GetPathPoints()
    {
        playerPathPoints.Insert(0, player.position);
        if (playerPathPoints.Count >= pathPositionsAmount+1)
        {
            playerPathPoints.RemoveAt(pathPositionsAmount);
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GetPathPoints());
    }
    private IEnumerator GetSurroundingTiles(Vector3Int _tile)
    {
        for (int x = 1; x > -2; x--)
        {
            for (int y = -1; y < 2; y++)
            {
                Vector3Int offset = new Vector3Int(x, y, 0);
                Vector3Int pos = _tile + offset;
                if (!groundTilemap.HasTile(pos) && !nightmareTilemap.HasTile(pos))
                {
                    Vector2 posVec2 = new Vector2(pos.x, pos.y);
                    float distanceToPlayer = Vector2.Distance(player.position, posVec2);
                    float f = Mathf.Max(0.002f * Mathf.Pow(distanceToPlayer, 2.0f), 1);
                    float timeToNext = maxTime / f;
                    for (int i = 0; i < playerPathPoints.Count; i++)
                    {
                        float distanceToPathpoint = Vector2.Distance(playerPathPoints[i], posVec2);
                        if (distanceToPathpoint < pathFollowDistance)
                        {
                            nightmareTilemap.SetTile(pos, nmTile);
                            yield return new WaitForSeconds(timeToNext * UnityEngine.Random.Range(1, 1.1f));
                            StartCoroutine(GetSurroundingTiles(pos));
                            break;
                        }
                    }
                }
            }
        }
    }
    private void OnDisable()
    {
        playerController.pHealth.e_PlayerDied -= ResetNightmare;
    }
}
