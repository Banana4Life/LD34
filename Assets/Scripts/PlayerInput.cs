using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    //public Camera camera;
    public GameObject trianglePrefab;

    private static Tile startTile;

    void OnMouseClick()
    {
        startTile = Tile.of(gameObject);
    }

    void OnMouseDrag()
    {
        // PathFinder.FindPath(startTile, Tile.of(gameObject), scheiße, scheiße);
    }

    void OnMouseUp()
    {
        Debug.Log(gameObject.transform);
    }
    /*
        void Update () {
            if (Input.GetMouseButtonDown(0))
            {
                var clickPos = camera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Linecast(clickPos, clickPos);
                if (hit2d)
                {
                    var triangle = Instantiate(trianglePrefab);
                    clickPos.z = 0;
                    triangle.transform.position = clickPos;
                    Debug.Log(hit2d.transform.name);
                }
            }

        }
    */

    /*
    TODO
       if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    */
}
