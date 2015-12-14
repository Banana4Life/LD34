using UnityEngine;
using System.Collections;

public abstract class TileObject : MonoBehaviour
{
    public double costToPass = 1d;
    public bool passable = true;

    public virtual bool canBePassed()
    {
        return passable;
    }

    public virtual double passingCost()
    {
        return costToPass;
    }
}
