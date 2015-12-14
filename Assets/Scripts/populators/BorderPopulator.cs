using UnityEngine;
using System.Collections;

public class BorderPopulator : TilePopulator
{
    public GameObject borderPrefab;
    public int borderStrength = 2;

    public override void populate(GameObject[,] gameObjects)
    {
        for (var x = 0; x < borderStrength; ++x)
        {
            for (var y = 0; y < borderStrength; ++y)
            {
                spawnBorder(gameObjects[x, y]);
                Debug.Log("X: " + x + ", Y: " + y);
            }
            for (var y = gameObjects.GetLength(1) - borderStrength; y < gameObjects.GetLength(1); ++y)
            {
                spawnBorder(gameObjects[x, y]);
                Debug.Log("X: " + x + ", Y: " + y);
            }
        }

        for (var y = borderStrength - 1; y < gameObjects.GetLength(1) - borderStrength; ++y)
        {
            for (var x = 0; x < borderStrength; ++x)
            {
                spawnBorder(gameObjects[x, y]);
                Debug.Log("X: " + x + ", Y: " + y);
            }
            for (var x = gameObjects.GetLength(0) - borderStrength; x < gameObjects.GetLength(0); ++x)
            {
                spawnBorder(gameObjects[x, y]);
                Debug.Log("X: " + x + ", Y: " + y);
            }
        }
    }

    protected void spawnBorder(GameObject g)
    {
        var border = Instantiate(borderPrefab);
        border.transform.parent = g.transform;
    }
}
