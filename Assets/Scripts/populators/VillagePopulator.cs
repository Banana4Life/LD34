using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class VillagePopulator : GridPopulator
{

    protected class Stats
    {
        private readonly float s2m;
        private readonly float m2l;

        int small = 0;
        int medium = 0;
        int large = 0;

        public Stats(float s2m, float m2l)
        {
            this.s2m = s2m;
            this.m2l = m2l;
        }

        public bool enoughSmall()
        {
            return small/s2m > medium;
        }

        public bool enoughMediam()
        {
            return medium/m2l > large;
        }

        public Size nextSize()
        {
            if (enoughSmall() && enoughMediam())
            {
                large++;
                return Size.CASTLE;
            }
            else if (enoughSmall())
            {
                medium++;
                return Size.VILLAGE;
            }
            else
            {
                small++;
                return Size.CAMP;
            }
        }

    }

    public GameObject villagePrefab;
    public GameObject smokePrefab;
    public float smallToMedium = 2f/1f;
    public float mediumToLarge = 5f/4f;
    public int tries = 500;

    public override void populate(GameObject[,] gameObjects)
    {
        var maxX = gameObjects.GetLength(0);
        var maxY = gameObjects.GetLength(1);

        var blocked = new HashSet<Tile>();
        var stats = new Stats(smallToMedium, mediumToLarge);

        for (var i = 0; i < tries; ++i)
        {
            var go = gameObjects[Random.Range(0, maxX), Random.Range(0, maxY)];
            var t = Tile.of(go);
            if (isValidSpace(t, blocked))
            {
                spawnVillage(go, stats);
                blocked.Add(t);
            }
        }

        populateFactions(gameObjects);
    }

    private void populateFactions(GameObject[,] gameObjects)
    {
        bool found = false;
        for (var x = 0; !found && x < gameObjects.GetLength(0); ++x)
        {
            for (var y = 0; y < gameObjects.GetLength(1); ++y)
            {
                var village = gameObjects[x, y].GetComponentInChildren<Village>();
                if (village && village.size == Size.CASTLE)
                {
                    found = true;
                    village.setFaction(Faction.FRIENDLY);
                    break;
                }
            }
        }

        found = false;
        for (var x = gameObjects.GetLength(0) - 1; !found && x >= 0; --x)
        {
            for (var y = 0; y < gameObjects.GetLength(1); ++y)
            {
                var village = gameObjects[x, y].GetComponentInChildren<Village>();
                if (village && village.size == Size.CASTLE)
                {
                    found = true;
                    village.setFaction(Faction.ENEMY);
                    break;
                }
            }
        }
    }

    public bool isValidSpace(Tile t, HashSet<Tile> blocked)
    {
        if (blocked.Contains(t))
        {
            return false;
        }
        if (t.fromChildren<TileObject>())
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

    protected void spawnVillage(GameObject tile, Stats stats)
    {
        var village = Instantiate(villagePrefab);
        village.transform.parent = tile.transform;
        village.transform.localPosition = new Vector3(0, 0, tile.transform.position.z - 0.5f);
        var v = village.GetComponent<Village>();
        v.setSize(stats.nextSize());

        var smoker = Instantiate(smokePrefab);
        smoker.transform.parent = village.transform;
        smoker.transform.localPosition = village.transform.localPosition;
        smoker.GetComponent<ParticleSystem>().playbackSpeed = 2;
    }

    protected bool isBigVillage(Tile t)
    {
        var v = t.GameObject.GetComponentInChildren<Village>();
        if (!v)
        {
            Debug.Log(v);
            return false;
        }
        return v.size == Size.CASTLE;
    }
}