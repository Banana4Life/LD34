using UnityEngine;
using System.Collections;

public class VillageCollision : MonoBehaviour {


    void OnCollisionEnter2D(Collision2D coll)
    {
        var parent = coll.collider.gameObject.transform.parent;
        if (parent == null)
        {
            return;
        }
        var walker = coll.collider.gameObject.transform.parent.gameObject.GetComponent<PathWalker>();
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

                var soundSpot = new Vector3(force.transform.position.x, force.transform.position.y, Camera.main.transform.position.z + 5.0f);

                if (village.faction != force.faction)
                {
                    if (!smoker.isPlaying) smoker.Play();
                    if ((force.faction == Faction.ENEMY && village.faction == Faction.FRIENDLY) ||
                        force.faction == Faction.FRIENDLY)
                    {
                        AudioSource.PlayClipAtPoint(force.deathSound, soundSpot, force.deathSoundVol);
                    }
                }
                else
                {
                    if (force.faction == Faction.FRIENDLY)
                    {
                        AudioSource.PlayClipAtPoint(force.arrivalSound, soundSpot, force.arrivalSoundVol);
                    }
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
