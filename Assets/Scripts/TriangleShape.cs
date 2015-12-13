﻿using UnityEngine;
using System.Collections;
using Math = System.Math;

public class TriangleShape : MonoBehaviour
{
    public Camera camera;
    public Mesh mesh;

    public GameObject legUnit1;

    private bool preparing = false;

    private Vector3 force;
    private Vector3 rate;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 pointC;

    private float toCenter;
    private float height;

    private Tile start;
    private Tile end;

    private static GameObject theTriangle;

    void Start()
    {
        if (theTriangle != null)
        {
            Destroy(theTriangle);
        }
        theTriangle = gameObject;

        this.camera = Camera.main;

        var scaling = (camera.orthographicSize / camera.pixelHeight / 2) * 450;
        var scale = new Vector3(scaling, scaling, scaling); ;
        gameObject.transform.localScale = scale;

        pointA = mesh.vertices[0]; 
        pointB = mesh.vertices[1];
        pointC = mesh.vertices[2];
        pointA.z = 0;
        pointB.z = 0;
        pointC.z = 0;
        pointA.Scale(scale);
        pointA += gameObject.transform.position;
        pointB.Scale(scale);
        pointB += gameObject.transform.position;
        pointC.Scale(scale);
        pointC += gameObject.transform.position;

        toCenter = (pointA - gameObject.transform.position).magnitude;
        var sideLength = (pointA - pointB).magnitude;
        height = (float)Math.Sqrt(toCenter * toCenter - (sideLength / 2 * sideLength / 2)) + toCenter;

        Debug.Log("Init Triangle: " + pointA + ":" + pointB + ":" + pointC + " - l=" + sideLength + " c = " + toCenter + " h=" + height + " sc=" + scaling);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pointA, toCenter);
        Gizmos.DrawWireSphere(pointB, toCenter);
        Gizmos.DrawWireSphere(pointC, toCenter);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pointA, height);
        Gizmos.DrawWireSphere(pointB, height);
        Gizmos.DrawWireSphere(pointC, height);
    }

    void Update()
    {
        if (preparing)
        {
            var pos = camera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -5;
            rate = new Vector3(toRate(pos, pointA), toRate(pos, pointB), toRate(pos, pointC));

            force += rate;

            if (force.x > 100000)
            {
                // TODO effect A
            }
            if (force.y > 100000)
            {
                // TODO effect B
            }
            if (force.z > 100000)
            {
                // TODO effect C
            }
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Preparing Attack...");
        preparing = true;
        force = Vector3.zero;

        var pos = camera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        rate = new Vector3(toRate(pos, pointA), toRate(pos, pointB), toRate(pos, pointC));
    }

    void OnMouseUp()
    {
        Debug.Log("Release the Legion! Force:" + force);
        preparing = false;
        Destroy(theTriangle);
        theTriangle = null;

        releaseLegion(force);
    }

    private float toRate(Vector3 mouse, Vector3 point)
    {
        var len = (mouse - point).magnitude;
        if (len <= toCenter)
        {
            return 100;
        }
        len = 100 * (height - len) / (height - toCenter);
        if (len < 0)
        {
            len = 0;
        }
        return len;
    }

    private void releaseLegion(Vector3 force)
    {
        Debug.Log(start.GameObject + " " + end.GameObject);
        var startVillage = start.GameObject.GetComponentInChildren<Village>();
        var endVillage = end.GameObject.GetComponentInChildren<Village>();

        Debug.Log(start.GameObject.transform.childCount + " " + end.GameObject.transform.childCount);
        var group = new GameObject("Legion Group");
        int amount = 50;
        for (int i = 0; i < amount; i++)
        {
            PathWalker.walk(spawn(legUnit1, startVillage.transform.position, group, amount / 50), start, end);
        }
    }

    public GameObject spawn(GameObject type, Vector3 at, GameObject inHere, int spread)
    {
        var unit = Instantiate(legUnit1);
        unit.transform.position = at + new Vector3((Random.value - 0.5f) * spread, (Random.value - 0.5f) * spread, 0);
        unit.transform.parent = inHere.transform;
        return unit;
    }

    public void init(Tile start, Tile end)
    {
        this.start = start;
        this.end = end;
    }
}
