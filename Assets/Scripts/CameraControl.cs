﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Camera camera;
    public float scrollspeed = 45;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float moveHorz = Input.GetAxis("Horizontal");
        float moveVert = Input.GetAxis("Vertical");

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        camera.orthographicSize -= scroll * 4;
        if (camera.orthographicSize < 3)
        {
            camera.orthographicSize = 3;
        }
        else if(camera.orthographicSize > 30)
        {
            camera.orthographicSize = 30;
        }

        Vector3 movement = new Vector3(-moveVert, moveHorz, 0);

        movement.Normalize();
        var scaling = scrollspeed * camera.orthographicSize / camera.pixelHeight / 2;
        movement.Scale(new Vector3(scaling, scaling, scaling));
        camera.transform.position = camera.transform.position + movement;
    }
}
