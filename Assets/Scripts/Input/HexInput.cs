using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexInput : MonoBehaviour {

    public GameObject trianglePrefab;

    public static List<Tile> startTiles = new List<Tile>();
    public static Tile endTile;
    public Material tileNormal;
    public Material tileHighlighted;
    public Material tileBlocked;
    public Material tileAllowed;

    private static GameObject line;

    private float force = 1;

    public static bool villageSelected()
    {
        return startTiles.Count > 0;
    }

    public void OnMouseDown()
    {
        if (startTiles.Count > 0)
        {
            if (endTile != null)
            {
                if (endTile.hasVillage())
                {
                    foreach (var startTile in startTiles)
                    {
                        startTile.getVillage().releaseLegion(new Vector3(force, force, force), startTile, endTile);
                    }
                }
            }
            startTiles.Clear();
            endTile = null;
            HexGrid.clearTilePaths(tileNormal);
            if (line != null)
            {
                Destroy(line);
                line = null;
            }
        }
        else
        {
            var startTile = Tile.of(gameObject);
            var village = gameObject.GetComponentInChildren<Village>();
            startTiles.Clear();
            HexGrid.clearTilePaths(tileNormal);
            if (!village)
            {
                startTiles.Clear();
            }
            else if (village.faction != Faction.FRIENDLY)
            {
                startTiles.Clear();
            }
            else
            {
                startTiles.Add(startTile);
                HexGrid.markTilePath(PathFinder.FindPath(startTile, startTile), tileHighlighted);
            }
        }

    }

    public void OnMouseEnter()
    {
        if (startTiles != null)
        {
            endTile = Tile.of(gameObject);
            HexGrid.clearTilePaths(tileNormal);
            foreach (var startTile in startTiles)
            {
                var theoreticalPath = PathFinder.FindPath(startTile, endTile);

                if (startTile == endTile)
                {
                    endTile = null;
                    HexGrid.markTilePath(theoreticalPath, tileHighlighted);
                }
                else
                {
                    gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

                    var color = tileBlocked;
                    if (endTile != null)
                    {
                        if (endTile.GameObject.transform.childCount > 0 && endTile.GameObject.transform.GetChild(0).GetComponent<Village>() != null)
                        {
                            color = tileAllowed;
                        }
                    }
                    HexGrid.markTilePath(theoreticalPath, color);
                }
            }
        }
    }

    void OnMouseExit()
    {
        HexGrid.clearTilePaths(tileNormal);
        if (line != null)
        {
            Destroy(line);
            line = null;
        }
    }
}
