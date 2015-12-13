using UnityEngine;
using System.Collections;
using Random = System.Random;

public class VillagePopulator : GridPopulator
{
    public GameObject villagePrefab;
    public int probability = 900;

    private static readonly Random random = new Random();

    public override void populate(GameObject[,] gameObjects)
    {
        for (int x = 0; x < gameObjects.GetLength(0); x++)
        {
            for (int y = 0; y < gameObjects.GetLength(1); y++)
            {
                var hex = gameObjects[x, y];

                if (random.Next(probability) <= 100 && hex.GetComponent<River>() == null)
                {
                    var village = Instantiate(villagePrefab);
                    village.transform.parent = hex.transform;
                    village.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}