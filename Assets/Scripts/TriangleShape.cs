using UnityEngine;
using System.Collections;
using Math = System.Math;

public class TriangleShape : MonoBehaviour
{
    public Camera camera;

    private bool preparing = false;

    private Vector3 force;
    private Vector3 rate;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 pointC;

    private float sideLength;
    private float height;

    void Start()
    {
        height = (float)Math.Sqrt(sideLength * sideLength - sideLength / 2 * sideLength / 2);
        // TODO init pointABC and sideLength
        sideLength = (pointA - pointB).magnitude;
    }

    void Update()
    {
        if (preparing)
        {
            force += rate;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Preparing Attack...");
        preparing = true;
    }

    void OnMouseUp()
    {
        Debug.Log("Release the Legion! Force:" + force);
        preparing = false;
    }

    void OnMouseMove()
    {
        if (preparing)
        {
            var pos = camera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;

            rate = new Vector3(toRate(pos, pointA), toRate(pos, pointB), toRate(pos, pointC));
        }
    }


    private float toRate(Vector3 mouse, Vector3 point)
    {
        var len = (mouse - point).magnitude;
        if (len > 0)
        {
            return 1;
        }
        return -len / (height - sideLength / 2);
    }
}
