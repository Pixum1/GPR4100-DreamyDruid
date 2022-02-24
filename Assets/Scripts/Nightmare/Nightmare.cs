using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Nightmare : MonoBehaviour
{
    [Header("Nightmare Values")]
    private PlayerController playerController;
    Transform player;
    [SerializeField]
    float maxTime;
    [SerializeField]
    int pathPositionsAmount = 20;
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
    public bool nmStarting;
    [SerializeField]
    Checkpoint startCheckpoint;
    [SerializeField]
    CrowAnimation crow;
    [Header("Cinematic")]
    [SerializeField]
    private float nightmareStartTime;
    [SerializeField]
    private RectTransform[] blackBars;
    [SerializeField]
    private float blackBarTime;
    [SerializeField]
    private GameObject animalGrid;
    [SerializeField]
    private Tilemap[] allTilemaps;
    [SerializeField]
    private Color nmColor;
    [SerializeField]
    private float colorChangeSpeed;
    int tileNumber;
    public float timer;
    private void Start()
    {
        timer = 0;
        playerController = FindObjectOfType<PlayerController>();
        cPManager = GameObject.Find("CheckPointManager").GetComponent<CheckpointManager>();
        playerController.pHealth.e_PlayerDied += new Action(ResetNightmare);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        active = false;
        StartCoroutine(GetPathPoints());
    }

    void Timer()
    {
        timer += Time.deltaTime;
    }

    private void Update()
    {
        if (active)
        {
            Timer();
        }
        currentCheckpoint = cPManager.currentCP;
        if (!active)
        {
            if (startCheckpoint == currentCheckpoint)
            {
                StartCoroutine(StartNightmare());
            }
        }
        if (PlayerPrefs.GetInt("DeathCount") > 10 && PlayerPrefs.GetInt("DeathCount") < 30)
            maxTime = .175f;

        else if (PlayerPrefs.GetInt("DeathCount") > 30)
            maxTime = .2f;
    }

    IEnumerator StartNightmare()
    {
        nmStarting = true;
        crow.active = true;
        active = true;
        playerPathPoints.Clear();
        #region Cinematic Stuff
        playerController.pMovement.movementInput = false;
        playerController.pHealth.canReset = false;
        animalGrid.SetActive(false);
        while (blackBars[0].rect.height < 100)
        {
            for (int i = 0; i < blackBars.Length; i++)
            {
                blackBars[i].sizeDelta = new Vector2(blackBars[i].sizeDelta.x, blackBars[i].sizeDelta.y + Time.unscaledDeltaTime * blackBarTime);
            }
            yield return null;
        }
        while (allTilemaps[0].color.g >= nmColor.g && allTilemaps[0].color.b >= nmColor.b)
        {
            for (int i = 0; i < allTilemaps.Length; i++)
            {
                allTilemaps[i].color = new Color(allTilemaps[i].color.r, allTilemaps[i].color.g - (Time.deltaTime * colorChangeSpeed), allTilemaps[i].color.b - (Time.deltaTime * colorChangeSpeed));
            }
            yield return null;
        }
        yield return new WaitForSeconds(nightmareStartTime / 3);
        playerController.pAnimation.playerSprite.flipX = true;
        yield return new WaitForSeconds(nightmareStartTime / 3);
        playerController.pAnimation.playerSprite.flipX = false;
        yield return new WaitForSeconds(nightmareStartTime / 3);

        playerController.pMovement.movementInput = true;
        playerController.pHealth.canReset = true;
        for (int j = 0; j < blackBars.Length; j++)
        {
            blackBars[j].sizeDelta = Vector2.zero;
        }
        animalGrid.SetActive(true);
        #endregion

        StartCoroutine(GetSurroundingTiles(startPosition));
        nmStarting = false;
        //StartCoroutine(GetSurroundingTiles(Vector3Int.RoundToInt(playerPathPoints[(int)nightmareStartTime*2])));
    }

    private void ResetNightmare()
    {
        if (active)
        {
            tileNumber = 0;
            StopAllCoroutines();
            nightmareTilemap.ClearAllTiles();
            currentCheckpoint = cPManager.currentCP;
            Vector3 resetPositonV3 = currentCheckpoint.transform.GetChild(0).transform.position;
            resetPosition = new Vector3Int(Mathf.RoundToInt(resetPositonV3.x), Mathf.RoundToInt(resetPositonV3.y), 0);
            playerPathPoints.Clear();
            StartCoroutine(GetPathPoints());
            StartCoroutine(GetSurroundingTiles(resetPosition));
        }
    }
    private IEnumerator GetPathPoints()
    {
        playerPathPoints.Insert(0, player.position);
        if (playerPathPoints.Count >= pathPositionsAmount + 1)
        {
            playerPathPoints.RemoveAt(pathPositionsAmount);
        }
        yield return new WaitForSeconds(.1f);
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
                    //Nightmare Speed Formula
                    float j = 2.5f;// Modify Speed quadratic (use with caution)
                    float l = 0.002f;// Modify Speed linear
                    float f = Mathf.Max(l * Mathf.Pow(distanceToPlayer, j), .8f);
                    float timeToNext = maxTime / f;
                    for (int i = 0; i < playerPathPoints.Count; i++)
                    {
                        float distanceToPathpoint = Vector2.Distance(playerPathPoints[i], posVec2);
                        if (distanceToPathpoint < pathFollowDistance)
                        {
                            nightmareTilemap.SetTile(pos, nmTile);
                            yield return new WaitForSeconds(timeToNext * UnityEngine.Random.Range(1, 1.1f));
                            tileNumber++;
                            StartCoroutine(GetSurroundingTiles(pos));
                            break;
                        }
                        else if (distanceToPathpoint < 30 && tileNumber < 200)
                        {
                            nightmareTilemap.SetTile(pos, nmTile);
                            yield return new WaitForSeconds(timeToNext * UnityEngine.Random.Range(1, 1.1f));
                            tileNumber++;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startPosition, Vector2.one);
    }
}
