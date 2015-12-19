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
        foreach (Transform child in gameObject.transform)
        {
            doMove(child.gameObject);
        }
    }

    void doMove(GameObject unit)
    {
        var target = this.enumerator.Current.GameObject.transform.position;

        var transform = unit.transform;
        var here = transform.position;

        var direction = (target - here);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        var rot = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed);

        var tileSpeed = this.speed * this.speedModifier;

        direction.z = here.z;
        transform.position += transform.up * -1 * tileSpeed;
        if (closeEnough(unit.transform.position, target))
        {
            this.enumerator.MoveNext(); // Destroy happens anyway when group is removed
        }
    }

    public static void walk(GameObject unit, Tile start, Tile end)
    {
        var walker = unit.AddComponent<PathWalker>();
        walker.followPath(PathFinder.FindPath<Tile>(start, end));
    }
}
