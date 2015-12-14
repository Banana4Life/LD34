using UnityEngine;
using System.Collections;

public class HexInput : MonoBehaviour {

    public GameObject trianglePrefab;

    public static Tile startTile;
    public static Tile endTile;
    public Material tileNormal;
    public Material tileHighlighted;
    public Material tileBlocked;
    public Material tileAllowed;

    private static GameObject line;
    private static Path<Tile> markedPath;

    private float force = 1;

    public static bool villageSelected()
    {
        return startTile != null;
    }

    public void OnMouseDown()
    {
        if (startTile != null)
        {
            if (endTile != null)
            {
                if (endTile.hasVillage())
                {
                    startTile.getVillage().releaseLegion(new Vector3(force, force, force), startTile, endTile);
                }
            }
            startTile = null;
            endTile = null;
            HexGrid.markTilePath(markedPath, tileNormal, tileNormal);
            if (line != null)
            {
                Destroy(line);
                line = null;
            }
        }
        else
        {
            startTile = Tile.of(gameObject);

            var village = gameObject.GetComponentInChildren<Village>();
            if (!village)
            {
                startTile = null;
            }
            else if (village.faction != Faction.FRIENDLY)
            {
                startTile = null;
            }
            else
            {
                var noPathYet = PathFinder.FindPath(startTile, startTile);
                HexGrid.markTilePath(noPathYet, tileNormal, tileHighlighted);
            }
        }

    }

    public void OnMouseEnter()
    {
        if (startTile != null)
        {
            endTile = Tile.of(gameObject);
            var theoreticalPath = PathFinder.FindPath(startTile, endTile);

            if (startTile == endTile)
            {
                endTile = null;
                HexGrid.markTilePath(theoreticalPath, tileNormal, tileHighlighted);
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

                markedPath = theoreticalPath;
                var color = tileBlocked;
                if (endTile != null)
                {
                    if (endTile.GameObject.transform.childCount > 0 && endTile.GameObject.transform.GetChild(0).GetComponent<Village>() != null)
                    {
                        color = tileAllowed;
                    }
                }
                HexGrid.markTilePath(markedPath, tileNormal, color);
            }
        }
    }

    void OnMouseExit()
    {
        if (markedPath != null)
        {
            HexGrid.markTilePath(markedPath, tileNormal, tileNormal);
        }
        if (line != null)
        {
            Destroy(line);
            line = null;
        }
    }
}
