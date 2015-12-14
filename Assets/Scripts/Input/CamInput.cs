using UnityEngine;
using System.Collections;

public class CamInput : MonoBehaviour {

    public Camera camera;
    private bool drag = false;
    private Vector3 dragOrigin;

    public float speed = 35;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Village>() == null)
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
}
