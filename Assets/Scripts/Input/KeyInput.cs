using UnityEngine;
using System.Collections;

public class KeyInput : MonoBehaviour {
	void Update () {
       if (Input.GetKey(KeyCode.Escape))
       {
           UIOverlay.pauseUnpause();
       }
    }
}
