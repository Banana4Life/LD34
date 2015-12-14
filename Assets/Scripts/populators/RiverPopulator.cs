using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class RiverPopulator : TilePopulator
{
    private static readonly Random random = new Random();
    public Material bentMat;
    public Material lakeMat;
    public GameObject riverPrefab;
    public Material straightMat;

    private Dictionary<string, string> snowflake = new Dictionary<string, string>();

    public RiverPopulator()
    {
        snowflake.Add(" 1|-1| 0|-1", "C|-60");
        snowflake.Add(" 1|-1| 1|-1", "S|-60");
        snowflake.Add(" 1|-1| 1| 0", "C|180");
        snowflake.Add(" 1| 0| 1|-1", "C|0");
        snowflake.Add(" 1| 0| 1| 0", "S|0");
        snowflake.Add(" 1| 0| 0| 1", "C|-120");
        snowflake.Add(" 0| 1| 1| 0", "C|60");
        snowflake.Add(" 0| 1| 0| 1", "S|60");
        snowflake.Add(" 0| 1|-1| 1", "C|-60");
        snowflake.Add("-1| 1| 0| 1", "C|120");
        snowflake.Add("-1| 1|-1| 1", "S|-60");
        snowflake.Add("-1| 1|-1| 0", "C|0");
        snowflake.Add("-1| 0|-1| 1", "C|180");
        snowflake.Add("-1| 0|-1| 0", "S|0");
        snowflake.Add("-1| 0| 0|-1", "C|60");
        snowflake.Add(" 0|-1|-1| 0", "C|-120");
        snowflake.Add(" 0|-1| 0|-1", "S|60");
        snowflake.Add(" 0|-1| 1|-1", "C|120");
    }

    private string getBendcase(int cx, int cy, int px, int py, int nx, int ny)
    {
        var diffs = new List<int>();
        diffs.Add(cx - px);
        diffs.Add(cy - py);
        diffs.Add(nx - cx);
        diffs.Add(ny - cy);
        var key = string.Join("|", diffs.Select(i => i.ToString().PadLeft(2)).ToArray());
        Debug.Log(key);

        var lookup = snowflake[key];
        return lookup == null ? "L|0" : lookup;
    }

    protected void spawnRiver(GameObject g)
    {
        var existingObject = g.GetComponentInChildren<TileObject>();
        if (existingObject)
        {
            Destroy(existingObject.gameObject);
        }
        var hexriver = Instantiate(riverPrefab);
        hexriver.transform.parent = g.transform;
        hexriver.transform.localPosition = Vector3.back;
    }

    public override void populate(GameObject[,] gameObjects)
    {
        var borderTiles = new List<GameObject>();

        for (var x = 0; x < gameObjects.GetLength(0); x++)
        {
            for (var y = 0; y < gameObjects.GetLength(1); y++)
            {
                if (x == 0 || y == 0 || x == gameObjects.GetLength(0) - 1 || y == gameObjects.GetLength(1) - 1)
                {
                    borderTiles.Add(gameObjects[x, y]);
                    //Debug.Log("added " + x + "," + y);
                }
            }
        }

        var path = new List<GameObject>();
        path.Add(borderTiles[random.Next(0, borderTiles.Count)]);
        //Debug.Log("rivered " + path[0].GetComponent<TileHolder>().tile);

        var blockedNeighbours = new List<GameObject>();


        //activate after first tile and exit when hitting next border tile
        do
        {
            var neighbours = path.Last().GetComponent<TileHolder>().tile.Neighbours.Select(n => n.GameObject);

            var allowedNeighbours =
                neighbours.Where(go => !path.Contains(go) && !blockedNeighbours.Contains(go)).ToList();
            if (path.Count == 1)
            {
                allowedNeighbours = allowedNeighbours.Where(go => !borderTiles.Contains(go)).ToList();
            }
            //lastNeighbours.Clear();
            blockedNeighbours.AddRange(neighbours);

            if (!allowedNeighbours.Any())
            {
                Debug.Log("lake please");
                break;
            }

            var nextTile = allowedNeighbours[random.Next(0, allowedNeighbours.Count)];


            path.Add(nextTile);
            spawnRiver(nextTile);

            //Debug.Log("rivered " + nextTile.GetComponent<TileHolder>().tile);
        } while (!borderTiles.Contains(path[path.Count - 1]));

        //HexGrid.drawPath(path, Color.blue, gameObject => gameObject.transform.position);

        for (var i = 0; i < path.Count; i++)
        {
            var tileHex = path[i];
            if (tileHex.transform.childCount == 0)
            {
                continue;
            }
            var riverHex = tileHex.GetComponentInChildren<River>().gameObject;

            //current, previous, next
            var c = tileHex.GetComponent<TileHolder>().tile;
            var p = i > 0 ? path[i - 1].GetComponent<TileHolder>().tile : null;
            var n = i < path.Count - 1 ? path[i + 1].GetComponent<TileHolder>().tile : null;


            var renderer = riverHex.GetComponent<MeshRenderer>();
            if (p != null && n != null)
            {
                var bendcase = getBendcase(c.X, c.Y, p.X, p.Y, n.X, n.Y);
                var rotation = int.Parse(bendcase.Substring(2));
                var type = bendcase.Substring(0, 1);

                switch (type)
                {
                    case "C":
                        renderer.material = bentMat;
                        break;
                    case "S":
                        renderer.material = straightMat;
                        break;
                    case "L":
                        renderer.material = lakeMat;
                        break;
                    default:
                        renderer.material = lakeMat;
                        Debug.LogWarning("no bendcase");
                        break;

                }

                var q = Quaternion.AngleAxis(-rotation, Vector3.forward);
                riverHex.transform.rotation = q;
                //riverHex.transform.localEulerAngles.Set(0, 0, rotation);
                //riverHex.transform.localRotation.Set(0,0,rotation,0);

                Debug.Log("(" + p.X + "," + p.Y + ")" + "->" + "(" + c.X + "," + c.Y + ")" + "->" + "(" + n.X + "," +
                          n.Y + ")" + " ~~> " + bendcase);
            }
            else
            {
                if (i == path.Count - 1 && i > 0)
                {
                    renderer.material = lakeMat;
                    var diff = (c.X - p.X) + "|" + (c.Y - p.Y);
                    int rotation = 180;
                    if (diff == "-1|0") rotation += 0;
                    if (diff == "0|-1") rotation += 60;
                    if (diff == "1|-1") rotation += 120;
                    if (diff == "1|0") rotation += 180;
                    if (diff == "0|1") rotation += 240;
                    if (diff == "-1|1") rotation += 300;
                    riverHex.transform.rotation = Quaternion.AngleAxis(-rotation, Vector3.forward);
                }
            }
        }


        /*        if (random.Next(probability) <= 100)
                {
                    var village = Instantiate(villagePrefab);
                    var hex = gameObjects[x, y];
                    village.transform.parent = hex.transform;
                    village.transform.localPosition = Vector3.zero;

                }*/
    }

    public void populateLake(GameObject[,] gameObjects, int xStart, int yStart)
    {
    }
}