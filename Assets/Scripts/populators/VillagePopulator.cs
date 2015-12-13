using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class VillagePopulator : GridPopulator
{
    public GameObject villagePrefab;

    public override void populate(GameObject[,] gameObjects)
    {
        var maxX = gameObjects.GetLength(0);
        var maxY = gameObjects.GetLength(1);

        var blocked = new HashSet<Tile>();

        for (var i = 0; i < 1000; ++i)
        {
            var go = gameObjects[Random.Range(0, maxX), Random.Range(0, maxY)];
            var t = Tile.of(go);
            if (isValidSpace(t, blocked))
            {
                spawnVillage(go);
                blocked.Add(t);
            }
        }
    }

    public bool isValidSpace(Tile t, HashSet<Tile> blocked)
    {
        if (blocked.Contains(t))
        {
            return false;
        }
        var N = 0;
        foreach (var n in t.Neighbours)
        {
            blocked.Add(n);
            N++;
        }
        if (N < 6)
        {
            return false;
        }
        return true;
    }

    public void spawnVillage(GameObject tile)
    {
        var village = Instantiate(villagePrefab);
        village.transform.parent = tile.transform;
        village.transform.localPosition = Vector3.zero;
    }

    private bool isBigVillage(Tile t)
    {
        var v = t.GameObject.GetComponentInChildren<Village>();
        if (!v)
        {
            Debug.Log(v);
            return false;
        }
        return v.size == Village.Size.CASTLE;
    }
}