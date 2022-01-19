using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileInfoTool : MonoBehaviour
{
    private Tilemap[] tM;

    private List<Vector3Int> usedTiles = new List<Vector3Int>();
    private List<Vector3Int> surroundingTiles = new List<Vector3Int>();
    private List<Vector3Int> emptyTiles = new List<Vector3Int>();

    private Color usedColor = new Color(1, 0, 0, 0.5f);
    private Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Color surroundingColor = new Color(0, 1, 0, 0.5f);

    private Vector3Int tMSize;
    private Vector3Int tMOrigin;

    private int tMWidth;
    private int tMHeight;
    private int index;

    private bool hasRun = false;

    [CustomEditor(typeof(TileInfoTool))]
    private class TileInfoToolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TileInfoTool IT = (TileInfoTool)target;

            GUILayout.Space(5f);
            if (IT.hasRun)
            {
                GUILayout.Label("Tile Info", EditorStyles.boldLabel);
                GUILayout.Space(10f);

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label("Used tiles", GUILayout.MaxWidth(Screen.width / 3));
                GUILayout.Label("Surrounding tiles", GUILayout.MaxWidth(Screen.width / 3));
                GUILayout.Label("Empty tiles", GUILayout.MaxWidth(Screen.width / 3));
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label(IT.usedTiles.Count.ToString(), GUILayout.MaxWidth(Screen.width / 3));
                GUILayout.Label(IT.surroundingTiles.Count.ToString(), GUILayout.MaxWidth(Screen.width / 3));
                GUILayout.Label(IT.emptyTiles.Count.ToString(), GUILayout.MaxWidth(Screen.width / 3));
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                IT.usedColor = EditorGUILayout.ColorField(IT.usedColor, GUILayout.MaxWidth(Screen.width / 3));
                IT.surroundingColor = EditorGUILayout.ColorField(IT.surroundingColor, GUILayout.MaxWidth(Screen.width / 3));
                IT.emptyColor = EditorGUILayout.ColorField(IT.emptyColor, GUILayout.MaxWidth(Screen.width / 3));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(20f);
            }

            if (GUILayout.Button("Run"))
            {
                IT.Run();
            }
        }
    }
    private void Run()
    {
        hasRun = true;
        index = 0;
        tM = Tilemap.FindObjectsOfType<Tilemap>();

        for (int i = 0; i < tM.Length; i++)
        {
            tMSize = tM[i].size;
            tMOrigin = tM[i].origin;
            tMWidth = tMSize.x;
            tMHeight = tMSize.y;
            GetTiles();
            index++;
        }
    }

    private void GetTiles()
    {
        for (int x = 0; x < tMWidth; x++)
        {
            for (int y = 0; y < tMHeight; y++)
            {
                Vector3Int pos = new Vector3Int(tMOrigin.x + x, tMOrigin.y + y, 0);
                if (tM[index].HasTile(pos))
                {
                    if (!usedTiles.Contains(pos))
                    {
                        usedTiles.Add(pos);
                        GetSurroundingTiles(pos);
                        surroundingTiles.Remove(pos);
                        emptyTiles.Remove(pos);
                    }
                }
                else
                {
                    if (!emptyTiles.Contains(pos) && !usedTiles.Contains(pos) && !surroundingTiles.Contains(pos))
                    {
                        emptyTiles.Add(pos);
                    }
                }
            }
        }
    }

    private void GetSurroundingTiles(Vector3Int _tile)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                Vector3Int offset = new Vector3Int(x, y, 0);
                Vector3Int pos = _tile + offset;

                if (!tM[index].HasTile(pos))
                {
                    if (!usedTiles.Contains(pos) && !surroundingTiles.Contains(pos))
                    {
                        surroundingTiles.Add(pos);
                        emptyTiles.Remove(pos);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = new Vector3(0.5f, 0.5f);
        Gizmos.color = usedColor;
        foreach (Vector3Int usedTile in usedTiles)
        {
            Gizmos.DrawCube(usedTile + offset, new Vector3(0.5f, 0.5f));
        }

        Gizmos.color = emptyColor;
        foreach (Vector3Int emptyTile in emptyTiles)
        {
            Gizmos.DrawCube(emptyTile + offset, new Vector3(0.5f, 0.5f));
        }

        Gizmos.color = surroundingColor;
        foreach (Vector3Int surroundingTile in surroundingTiles)
        {
            Gizmos.DrawCube(surroundingTile + offset, new Vector3(0.5f, 0.5f));
        }
    }
}
