using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathWalker : MonoBehaviour
{
    public IEnumerable<Tile> path;
    private Tile currentGoal;
    private float minimumDistance = 4f;

	// Use this for initialization
	void Start () {
	
	}

    public void followPath(IEnumerable<Tile> path, bool invert = true)
    {
        this.path = invert ? path.Reverse() : path;
        this.currentGoal = path.First();
    }

    private bool closeEnough(Vector3 v1, Vector3 v2)
    {
        return (v2 - v1).sqrMagnitude <= (minimumDistance * minimumDistance);
    }

    // Update is called once per frame
	void FixedUpdate()
	{
	    var here = gameObject.transform.position;
	    var target = this.currentGoal.GameObject.transform.position;

	    var direction = (target - here).normalized;
        gameObject.transform.position += direction;
	}
}
