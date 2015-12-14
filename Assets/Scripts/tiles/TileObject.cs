using UnityEngine;
using System.Collections;

public abstract class TileObject : MonoBehaviour
{
    public double costToPass = 1d;
    public bool passable = true;
    public float speedFactor = 1f;

    public virtual bool canBePassed()
    {
        return passable;
    }

    public virtual double getCostToPass()
    {
        return costToPass;
    }

    public virtual float getSpeedFactor()
    {
        return speedFactor;
    }
}
