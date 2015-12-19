using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexInput : MonoBehaviour {

    public GameObject trianglePrefab;

    public AudioClip clickSound;
    public float clickSoundVol;

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
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position, clickSoundVol);
        UIOverlay.scrollUi.SetActive(false);
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
                UIOverlay.scrollUi.SetActive(true);
                CameraScroll.updateScrollUi();
                startTiles.Add(startTile);
                HexGrid.markTilePath(PathFinder.FindPath(startTile, startTile), tileHighlighted);
            }
        }
    }

    public void OnMouseUp()
    {
        OnMouseEnter();
    }

    public void OnMouseEnter()
    {
        if (startTiles.Count > 0)
        {
            endTile = Tile.of(gameObject);

            HexGrid.clearTilePaths(tileNormal);

            Path<Tile> selfPath = null;
            foreach (var startTile in startTiles)
            {
                var theoreticalPath = PathFinder.FindPath(startTile, endTile);

                if (startTile != endTile)
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
                    else
                    {
                        selfPath = PathFinder.FindPath(endTile, endTile);
                    }
                    HexGrid.markTilePath(theoreticalPath, color);
                }
            }
            if (startTiles.Contains(endTile))
            {
                HexGrid.markTilePath(PathFinder.FindPath(endTile, endTile), tileHighlighted);
                if (startTiles.Count == 1)
                {
                    endTile = null;
                }
                return;
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
