
using UnityEngine;

public abstract class GridPopulator : MonoBehaviour
{
    abstract public void populate(GameObject[,] gameObjects);
}