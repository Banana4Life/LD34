using UnityEngine;
using System.Collections;

public abstract class TileObject : MonoBehaviour {
    public virtual bool canBePassed()
    {
        return true;
    }

    public virtual double passingCost()
    {
        return 1d;
    }
}
