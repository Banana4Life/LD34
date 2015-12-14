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
        }

    }

    public void OnMouseEnter()
    {
        if (startTile != null)
        {
            endTile = Tile.of(gameObject);

            if (startTile == endTile)
            {
                endTile = null;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

                markedPath = PathFinder.FindPath(startTile, endTile);
                //line = HexGrid.drawPath(markedPath, Color.yellow, t => t.GameObject.transform.position);
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
