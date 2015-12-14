using UnityEngine;
using System.Collections;

public class CamInput : MonoBehaviour {

    public Camera camera;
    public GameObject grid;

    private bool drag = false;
    private Vector3 dragOrigin;

    public float speed = 35;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit.collider != null)
            {
                drag = true;
                dragOrigin = Input.mousePosition;
            }
        }

        if (!Input.GetMouseButton(1))
        {
            drag = false;
            return;
        }

        if (drag)
        {
            var mov = camera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            dragOrigin = Input.mousePosition;
            mov = new Vector3(-mov.x * speed, -mov.y * speed, 0);
            camera.transform.Translate(mov);
        }
    }

    void OnMouseUp()
    {
        drag = false;
    }

    void LateUpdate()
    {
        var hexGrid = grid.GetComponent<HexGrid>();

        var camPos = camera.transform.position;

        var size = camera.orthographicSize;
        if (hexGrid.maxY - hexGrid.minY > size)
        {
            camPos.y = Mathf.Clamp(camPos.y, hexGrid.minY + size, hexGrid.maxY - size);
        }
        size = size * camera.aspect;
        if (hexGrid.maxX - hexGrid.minX > size)
        {
            camPos.x = Mathf.Clamp(camPos.x, hexGrid.minX + size, hexGrid.maxX - size);
        }
        camera.transform.position = camPos;
    }

    void OnDrawGizmos()
    {
        var hexGrid = grid.GetComponent<HexGrid>();
        var size = camera.orthographicSize;
        Gizmos.color = Color.red;
//outer
        Gizmos.DrawLine(new Vector3(hexGrid.minX, hexGrid.minY, 0), new Vector3(hexGrid.maxX, hexGrid.minY, 0));
        Gizmos.DrawLine(new Vector3(hexGrid.minX, hexGrid.maxY, 0), new Vector3(hexGrid.maxX, hexGrid.maxY, 0));
//inner
        Gizmos.DrawLine(new Vector3(hexGrid.minX, hexGrid.minY + size, 0), new Vector3(hexGrid.maxX, hexGrid.minY + size, 0));
        Gizmos.DrawLine(new Vector3(hexGrid.minX, hexGrid.maxY - size, 0), new Vector3(hexGrid.maxX, hexGrid.maxY - size, 0));

        Gizmos.color = Color.yellow;
//outer
        Gizmos.DrawLine(new Vector3(hexGrid.minX, hexGrid.minY, 0), new Vector3(hexGrid.minX, hexGrid.maxY, 0));
        Gizmos.DrawLine(new Vector3(hexGrid.maxX, hexGrid.minY, 0), new Vector3(hexGrid.maxX, hexGrid.maxY, 0));
//inner
        size = size * camera.aspect;
        Gizmos.DrawLine(new Vector3(hexGrid.minX + size, hexGrid.minY, 0), new Vector3(hexGrid.minX + size, hexGrid.maxY, 0));
        Gizmos.DrawLine(new Vector3(hexGrid.maxX - size, hexGrid.minY, 0), new Vector3(hexGrid.maxX - size, hexGrid.maxY, 0));

    }
}
