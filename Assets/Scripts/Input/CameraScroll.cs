using UnityEngine;
using System.Collections;
using System.Net;
using System.Xml.Serialization;
using UnityEngine.UI;
using Math = System.Math;

public class CameraScroll : MonoBehaviour {

    public Camera camera;
    public static GameObject scrollUi;
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
                Village.percent += 10;
            }
            else if (scroll < 0)
            {
                Village.percent -= 10;
            }
            else
            {
                return;
            }
            if (Village.percent > 100)
            {
                Village.percent = 100;
            }
            if (Village.percent < 10)
            {
                Village.percent = 10;
            }
            Debug.Log(Village.percent);
            updateScrollUi();
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

    public static void updateScrollUi()
    {
        int bars = (int)Village.percent / 10;
        for (int i = 0; i < 10; i++)
        {
            scrollUi.transform.GetChild(i).gameObject.SetActive(i < bars);
        }
        scrollUi.transform.GetChild(10).GetComponent<Text>().text = Village.percent + "%";
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
