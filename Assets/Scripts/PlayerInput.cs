using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    public Camera camera;

    public GameObject trianglePrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
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
}
