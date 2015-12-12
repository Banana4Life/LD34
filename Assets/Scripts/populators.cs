
using UnityEngine;

public interface GridPopulator
{
    void populate(GameObject[,] gameObjects);
}

class VillagePopulator : GridPopulator
{
    public void populate(GameObject[,] gameObjects)
    {

    }
}

class RiverPopulator : GridPopulator
{
    public void populate(GameObject[,] gameObjects)
    {

    }
}
