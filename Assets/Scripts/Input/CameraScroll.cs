using UnityEngine;
using System.Collections;
using Math = System.Math;

public class CameraScroll : MonoBehaviour {

    public Camera camera;
    // public float moveSpeed = 45;

	void Update () {
        movement();
        scroll();
    }

    private void scroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        camera.orthographicSize -= scroll * 4;
        if (camera.orthographicSize < 3)
        {
            camera.orthographicSize = 3;
        }
        else if (camera.orthographicSize > 30)
        {
            camera.orthographicSize = 30;
        }
    }

    private void movement()
    {
        /*
        float moveHorz = Input.GetAxis("Horizontal");
        float moveVert = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorz, moveVert, 0);

        var scaling = scrollspeed * (camera.orthographicSize / camera.pixelHeight / 2) * (float)Math.Pow(movement.magnitude, 1 / 3);
        movement.Scale(new Vector3(scaling, scaling, scaling));
        camera.transform.position = camera.transform.position + movement;
        //*/
    }
}
