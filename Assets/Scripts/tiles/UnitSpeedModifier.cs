using UnityEngine;
using System.Collections;

public class UnitSpeedModifier : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collider)
    {
        var walker = collider.gameObject.GetComponent<PathWalker>();
        if (!walker)
        {
            return;
        }
        var tileObject = gameObject.GetComponentInChildren<TileObject>();
        walker.setSpeedModifier(tileObject ? tileObject.speedFactor : 1f);
    }
}
