using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Camera camera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float moveHorz = Input.GetAxis("Horizontal");
        float moveVert = Input.GetAxis("Vertical");

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        camera.orthographicSize -= scroll * 4;
        if (camera.orthographicSize < 2)
        {
            camera.orthographicSize = 2;
        }
        else if(camera.orthographicSize > 10)
        {
            camera.orthographicSize = 10;
        }

        Vector3 movement = new Vector3(-moveVert, moveHorz, 0);
        movement.Scale(new Vector3(1/camera.orthographicSize * 1.5f, 1/camera.orthographicSize * 1.5f, 1/camera.orthographicSize * 1.5f));
        camera.transform.position = camera.transform.position + movement;
    }
}
