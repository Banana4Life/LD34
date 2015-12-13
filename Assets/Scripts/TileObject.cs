using UnityEngine;
using System.Collections;

public abstract class TileObject : MonoBehaviour {
    public virtual bool canBePassed()
    {
        return true;
    }
}
