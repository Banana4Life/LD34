using UnityEngine;
using System.Collections;
using Random = System.Random;

public class VillagePopulator : GridPopulator
{
    public GameObject villagePrefab;

    private static readonly Random random = new Random();

    override
    public void populate(GameObject[,] gameObjects)
    {
        for (int x = 0; x < gameObjects.GetLength(0); x++)
        {
            for (int y = 0; y < gameObjects.GetLength(1); y++)
            {
                if (random.Next(900) <= 100)
                {
                    var village = Instantiate(villagePrefab);
                    var hex = gameObjects[x, y];
                    village.transform.parent = hex.transform;
                    village.transform.localPosition = Vector3.zero;
                    var hasVillage = hex.AddComponent<HasVillage>();
                    hasVillage.village = village;
                }
            }
        }
    }
}