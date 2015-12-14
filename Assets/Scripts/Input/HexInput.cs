﻿using UnityEngine;
using System.Collections;

public class HexInput : MonoBehaviour {

    public GameObject trianglePrefab;

    private static Tile startTile;
    private static Tile endTile;
    public Material tileNormal;
    public Material tileHighlighted;
    public Material tileBlocked;
    public Material tileAllowed;

    private static GameObject line;
    private static Path<Tile> markedPath;

    private float force = 1;

    public GameObject legUnit1;
    public GameObject legUnit2;
    public GameObject legUnit3;

    void OnMouseDown()
    {
        if (startTile != null)
        {
            if (endTile != null)
            {
                if (endTile.hasVillage())
                {
                    releaseLegion(new Vector3(force, force, force), startTile, endTile);
                }
            }
            startTile = null;
            endTile = null;
            HexGrid.markTilePath(markedPath, tileNormal);
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

    void OnMouseEnter()
    {
        if (startTile != null)
        {
            endTile = Tile.of(gameObject);

            if (startTile == endTile)
            {
                endTile = null;
            }

            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

            markedPath = PathFinder.FindPath(startTile, Tile.of(gameObject));
            //line = HexGrid.drawPath(markedPath, Color.yellow, t => t.GameObject.transform.position);
            var color = tileBlocked;
            if (endTile != null)
            {
                if (endTile.GameObject.transform.childCount > 0 && endTile.GameObject.transform.GetChild(0).GetComponent<Village>() != null)
                {
                    color = tileAllowed;
                }
            }
            HexGrid.markTilePath(markedPath, color);
        }
    }

    void OnMouseExit()
    {
        HexGrid.markTilePath(markedPath, tileNormal);
        if (line != null)
        {
            Destroy(line);
            line = null;
        }
    }

    private void releaseLegion(Vector3 force, Tile start, Tile end)
    {
        Debug.Log("Release the Legion! Force:" + force + " " + start.GameObject.transform.position + "->" + end.GameObject.transform.position);
        var startVillage = start.getVillage();
        var endVillage = end.getVillage();

        Debug.Log(start.GameObject.transform.childCount + " " + end.GameObject.transform.childCount);
        var group = new GameObject("Legion Group");
        int amount = 50;
        for (int i = 0; i < amount; i++)
        {
            switch ((int)(Random.value * 3))
            {
                // TODO
                case 0:
                    PathWalker.walk(spawn(legUnit1, startVillage.transform.position, group, amount / 50), start, end);
                    break;
                case 1:
                    PathWalker.walk(spawn(legUnit2, startVillage.transform.position, group, amount / 50), start, end);
                    break;
                case 2:
                    PathWalker.walk(spawn(legUnit3, startVillage.transform.position, group, amount / 50), start, end);
                    break;
            }
        }
    }

    public GameObject spawn(GameObject type, Vector3 at, GameObject inHere, int spread)
    {
        var unit = Instantiate(type);
        unit.transform.position = at + new Vector3((Random.value - 0.5f) * spread, (Random.value - 0.5f) * spread, 0);
        unit.transform.parent = inHere.transform;
        return unit;
    }
}
