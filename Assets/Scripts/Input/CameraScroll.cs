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

    public float minScroll = 5;
    public float maxScroll = 15;

    private void scroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (HexInput.villageSelected())
        {
            if (scroll > 0)
            {
                Village.percent += 5;
            }
            else if (scroll < 0)
            {
                Village.percent -= 5;
            }
            else
            {
                return;
            }
            if (Village.percent > 100)
            {
                Village.percent = 100;
            }
            if (Village.percent < 0)
            {
                Village.percent = 0;
            }
            Debug.Log(Village.percent);
            return;
        }

        camera.orthographicSize -= scroll * 4;
        if (camera.orthographicSize < minScroll)
        {
            camera.orthographicSize = minScroll;
        }
        else if (camera.orthographicSize > maxScroll)
        {
            camera.orthographicSize = maxScroll;
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
