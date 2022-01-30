using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class Nightmare : MonoBehaviour
{
    //[SerializeField]
    //LineRenderer lr;

    //[SerializeField]
    //Tilemap tilemap;

    Transform player;

    [SerializeField]
    float maxTime;
    //[SerializeField]
    //float step;

    //Vector3 endPos;

    [SerializeField]
    List<Vector3Int> emptyNightmareTiles = new List<Vector3Int>();
    [SerializeField]
    List<Vector3Int> usedNightmareTiles = new List<Vector3Int>();
    [SerializeField]
    Vector3Int startPos;

    [SerializeField]
    int i;

    [SerializeField]
    Tilemap nightmareTilemap;

    [SerializeField]
    TileBase[] nmTile;

    TileInfoTool tIT;

    private void Start()
    {
        tIT = GetComponent<TileInfoTool>();
        emptyNightmareTiles = tIT.LoadTileData().surroundingTilesData;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        usedNightmareTiles.Add(startPos);
        emptyNightmareTiles.Remove(startPos);
        GetNextTile();

        //StartCoroutine("Spawn");        
    }

    //public Vector3 GetEndPosition(Transform _target)
    //{
    //    Vector3 targetPos = _target.position;

    //    float dist = Vector3.Distance(endPos, targetPos);

    //    endPos = Vector3.MoveTowards(endPos, targetPos, step * dist);

    //    return endPos;
    //}

    //public Vector3Int GetStartPosition()
    //{
    //    int posY = 0;

    //    int posX = 0;

    //    Vector3Int pos = new Vector3Int(posX, posY, 0);

    //    return pos;
    //}

    //private IEnumerator Spawn()
    //{
    //    if (!tilemap.HasTile(GetStartPosition()))
    //    {
    //        lr.SetPosition(0, GetStartPosition());
    //        lr.SetPosition(1, GetEndPosition(player));
    //    }
    //    yield return new WaitForSeconds(seconds);
    //    StartCoroutine("Spawn");
    //}

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

                if (emptyNightmareTiles.Contains(pos))
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
        
        nightmareTilemap.SetTiles(bufferTiles.ToArray(),nmTile);
        GetNextTile();
    }

    //private void OnDrawGizmos()
    //{
    //    Vector3 offset = new Vector3(0.5f, 0.5f);
    //    Gizmos.color = Color.red;
    //    foreach (Vector3Int usedNightmareTile in usedNightmareTiles)
    //    {
    //        Gizmos.DrawCube(usedNightmareTile + offset, new Vector3(0.5f, 0.5f));
    //    }
    //}
}
