using UnityEngine;
using System.Collections;

public class VillageCollision : MonoBehaviour {


    void OnCollisionEnter2D(Collision2D coll)
    {
        var end = coll.collider.gameObject.GetComponent<PathWalker>().getEnd();
        if (Tile.of(gameObject.transform.parent.gameObject) == end)
        {
            Force force = coll.collider.gameObject.GetComponent<Force>();
            var group = coll.collider.gameObject.transform.parent;
            if (group.childCount <= 1)
            {
                Destroy(group.gameObject);
                gameObject.GetComponent<Village>().fight(force.force, force.faction);
            }
            else
            {
                Destroy(coll.collider.gameObject);
                gameObject.GetComponent<Village>().fight(force.force, force.faction);
            }
        }
    }

    void OnMouseDown()
    {
        gameObject.transform.parent.GetComponent<HexInput>().OnMouseDown();
    }

    void OnMouseEnter()
    {
        gameObject.transform.parent.GetComponent<HexInput>().OnMouseEnter();
    }
}
