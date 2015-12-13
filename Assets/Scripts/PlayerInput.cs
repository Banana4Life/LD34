using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    public Camera camera;
    public GameObject trianglePrefab;

    private static Tile startTile;
    private static Tile endTile;

    private static GameObject line;

    void OnMouseDown()
    {
        startTile = Tile.of(gameObject);

        var village = gameObject.GetComponentInChildren<Village>();
        if (!village)
        {
            startTile = null;
        }

        if (village.faction != Village.Faction.FRIENDLY)
        {
            startTile = null;
        }
    }

    void OnMouseEnter()
    {
        if (startTile != null)
        {
            endTile = Tile.of(gameObject);

            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

            var path = PathFinder.FindPath(startTile, Tile.of(gameObject));
            line = HexGrid.drawPath(path, Color.yellow, t => t.GameObject.transform.position);
        }
    }

    void OnMouseExit()
    {
        if (line != null)
        {
            Destroy(line);
            line = null;
        }
    }

    void OnMouseUp()
    {
        if (endTile != null)
        {
            if (endTile.GameObject.GetComponentInChildren<Village>())
            {
                var triangle = Instantiate(trianglePrefab);
                var pos = endTile.GameObject.transform.position;
                pos.z = -5;
                triangle.transform.position = pos;
                triangle.transform.parent = startTile.GameObject.transform;

                triangle.GetComponent<TriangleShape>().init(startTile, endTile);
            }
        }
        startTile = null;
        endTile = null;
        if (line != null)
        {
            Destroy(line);
            line = null;
        }
    }
}
