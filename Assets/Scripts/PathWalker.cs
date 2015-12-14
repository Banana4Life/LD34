using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathWalker : MonoBehaviour
{
    public IEnumerable<Tile> path;
    private IEnumerator<Tile> enumerator;
    private float minimumDistance = 1f;
    private float speed = 0.08f;
    private float speedModifier = 1f;
    private float rotationSpeed = 0.11f;

    private float avoidRecover = 0;

    public void followPath(IEnumerable<Tile> path, bool invert = true)
    {
        this.path = invert ? path.Reverse() : path;
        this.enumerator = this.path.GetEnumerator();
        this.enumerator.MoveNext();
    }

    internal void setSpeedModifier(float s)
    {
        this.speedModifier = s;
    }

    public Tile getEnd()
    {
        return path.Last<Tile>();
    }

    private bool closeEnough(Vector3 v1, Vector3 v2)
    {
        return (v2 - v1).sqrMagnitude <= (minimumDistance * minimumDistance);
    }

    void FixedUpdate()
	{
	    var transform = gameObject.transform;
        var here = transform.position;
        var target = this.enumerator.Current.GameObject.transform.position;

        var direction = (target - here);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        var rot = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed);

	    var tileSpeed = this.speed * this.speedModifier;

	    direction.z = here.z;
        transform.position += transform.up * -1 * tileSpeed;
	    if (closeEnough(gameObject.transform.position, target))
	    {
	        if (!this.enumerator.MoveNext())
	        {
	            Destroy(this);
	        }
	    }


        if (avoidRecover > 0)
        {
            avoidRecover -= Time.fixedDeltaTime;
        }
        else
        {
            foreach (Transform child in transform.parent)
            {
                if (transform != child)
                {
                    if ((transform.position - child.position).sqrMagnitude < 1)
                    {
                        var childWalker = child.GetComponent<PathWalker>();
                        if (childWalker && childWalker.avoidRecover <= 0)
                        {
                            var randRot = transform.rotation;
                            randRot.z += (Random.value - 0.5f) / 7;
                            transform.rotation = randRot;

                            if (avoidRecover <= 0)
                            {
                                avoidRecover = (Random.value);
                                tileSpeed = 0.08f - Random.value / 500;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    public static void walk(GameObject unit, Tile start, Tile end)
    {
        var walker = unit.AddComponent<PathWalker>();
        walker.followPath(PathFinder.FindPath<Tile>(start, end));
    }
}
