using UnityEngine;
using System.Collections;

public class CamInput : MonoBehaviour {

    public Camera camera;
    public GameObject grid;

    private bool drag = false;
    private Vector3 dragOrigin;

    public float speed = 35;

    private bool boxDrag = false;
    private Vector3 boxDragOrigin;

    public GameObject boxPrefab;

    void Update()
    {
        handleRight();
        handleLeft();
    }

    private void handleLeft()
    {
        if (!HexInput.villageSelected())
        {

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("left D");

                boxDrag = true;
                boxDragOrigin = Input.mousePosition;
            }
            if (boxDrag && !Input.GetMouseButton(0))
            {
                Debug.Log("left U");
                HexInput.startTiles.Clear();
                foreach (var village in HexGrid.villages)
                {
                    if (village.GetComponent<Village>().faction == Faction.FRIENDLY && IsWithinSelectionBounds(village))
                    {
                        HexInput.scrollUi.SetActive(true);
                        CameraScroll.updateScrollUi();
                        HexInput.startTiles.Add(Tile.of(village.transform.parent.gameObject));
                    }
                }
                boxDrag = false;
                return;
            }
            return;
        }
        boxDrag = false;
    }

    private void handleRight()
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

    void OnGUI()
    {
        if (boxDrag)
        {
            var v1 = new Vector2(boxDragOrigin.x, Screen.height - boxDragOrigin.y);
            var boxDragTarget = Input.mousePosition;
            var v2 = new Vector2(boxDragTarget.x, Screen.height - boxDragTarget.y);

            var topLeft = Vector2.Min(v1, v2);
            var bottomRight = Vector2.Max(v1, v2);

            var rect = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!boxDrag)
            return false;

        var camera = Camera.main;
        var viewportBounds =
            Utils.GetViewportBounds(camera, boxDragOrigin, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(gameObject.transform.position));
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
