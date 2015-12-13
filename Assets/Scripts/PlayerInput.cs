using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    //public Camera camera;
    public GameObject trianglePrefab;

    private static Tile startTile;
    private static Tile endTile;

    void OnMouseDown()
    {
        startTile = Tile.of(gameObject);
        Debug.Log("CLICK " + gameObject.name + " - " + gameObject.transform.position
            + " ### (" + gameObject.transform.childCount + ")" + gameObject.GetComponentInChildren<Village>()
            + " ### (" + startTile.X + "x" + startTile.Y + ")"
            );
        
        if (!gameObject.GetComponentInChildren<Village>())
        {
            startTile = null;
        }
    }

    void OnMouseEnter()
    {
        if (startTile != null)
        {
            endTile = Tile.of(gameObject);
            Debug.Log("ENTER " + gameObject.transform.position + " ### " + endTile.X + "x" + endTile.Y);

            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

            var path = PathFinder.FindPath(startTile, Tile.of(gameObject));
            HexGrid.drawPath(path, Color.yellow, t => t.GameObject.transform.position);
        }
    }

    void OnMouseLeave()
    {
        // TODO delete prev. path if present
    }

    void OnMouseUp()
    {
        if (endTile != null)
        {
            Debug.Log("UP " + endTile.GameObject.transform.position);
        }
        startTile = null;
        endTile = null;
    }
    /*
    TODO
       if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    */
}
