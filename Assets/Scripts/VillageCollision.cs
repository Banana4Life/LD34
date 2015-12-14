using UnityEngine;
using System.Collections;

public class VillageCollision : MonoBehaviour {


    void OnCollisionEnter2D(Collision2D coll)
    {
        var walker = coll.collider.gameObject.GetComponent<PathWalker>();
        if (!walker)
        {
            return;
        }
        var end = walker.getEnd();
        if (Tile.of(gameObject.transform.parent.gameObject) == end)
        {
            Force force = coll.collider.gameObject.GetComponent<Force>();
            var group = coll.collider.gameObject.transform.parent;
                var smoker = gameObject.GetComponentInChildren<ParticleSystem>();
            if (group.childCount <= 1)
            {
                Destroy(group.gameObject);
                gameObject.GetComponent<Village>().fight(force.force, force.faction);
                if (smoker.isPlaying) smoker.Stop();
            }
            else
            {

                Destroy(coll.collider.gameObject);
                var village = gameObject.GetComponent<Village>();
                village.fight(force.force, force.faction);
                if (village.faction != Faction.FRIENDLY)
                {
                    if (!smoker.isPlaying) smoker.Play();
                    AudioSource.PlayClipAtPoint(force.deathSound, force.transform.position, force.deathSoundVol);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(force.arrivalSound, force.transform.position, force.arrivalSoundVol);
                }
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
