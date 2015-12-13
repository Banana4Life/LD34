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
    private float rotationSpeed = 0.11f;

    public void followPath(IEnumerable<Tile> path, bool invert = true)
    {
        this.path = invert ? path.Reverse() : path;
        this.enumerator = this.path.GetEnumerator();
        this.enumerator.MoveNext();
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

	    direction.z = here.z;
        transform.position += transform.up * -1 * speed;
	    if (closeEnough(gameObject.transform.position, target))
	    {
	        if (!this.enumerator.MoveNext())
	        {
	            Destroy(this);
	        }
	    }
	}

    public static void walk(GameObject unit, Tile end)
    {
        var walker = unit.AddComponent<PathWalker>();
        var list = new List<Tile>();
        list.Add(end);
        walker.followPath(list);
    }
}
