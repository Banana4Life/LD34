using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;
using System.Linq;

public class RiverPopulator : GridPopulator
{
    private static readonly Random random = new Random();

    override
    public void populate(GameObject[,] gameObjects)
    {
        List<GameObject> borderTiles = new List<GameObject>();

        for (int x = 0; x < gameObjects.GetLength(0); x++)
        {
            for (int y = 0; y < gameObjects.GetLength(1); y++)
            {
                if (x == 0 || y == 0 || x == gameObjects.GetLength(0)-1 || y == gameObjects.GetLength(1)-1)
                {
                    borderTiles.Add(gameObjects[x, y]);
                    //Debug.Log("added " + x + "," + y);
                }
            }
        }

        List<GameObject> path = new List<GameObject>();
        path.Add(borderTiles[random.Next(0, borderTiles.Count)]);
        Debug.Log("rivered " + path[0].GetComponent<TileBehaviour>().tile);

        Debug.Log(path.Contains(path[0]));
        List<GameObject> blockedNeighbours = new List<GameObject>();


        //activate after first tile and exit when hitting next border tile
        do
        {
            var neighbours = path.Last().GetComponent<TileBehaviour>().tile.Neighbours.Select(n => n.GameObject);

            var allowedNeighbours = neighbours.Where(go => !path.Contains(go) && !blockedNeighbours.Contains(go)).ToList();
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

            GameObject nextTile = allowedNeighbours[random.Next(0, allowedNeighbours.Count)];

            path.Add(nextTile);
            Debug.Log("rivered " + nextTile.GetComponent<TileBehaviour>().tile);
        } while (!borderTiles.Contains(path[path.Count - 1]));

        HexGrid.drawPath(path, Color.blue, gameObject => gameObject.transform.position);
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