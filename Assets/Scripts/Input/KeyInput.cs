using UnityEngine;
using System.Collections;

public class KeyInput : MonoBehaviour
{
    private bool esc = false;

	void Update () {
	    if (Input.GetKey(KeyCode.Escape))
	    {
	        if (!esc)
	        {
	            UIOverlay.pauseUnpause();
	            esc = true;
	        }
	    }
	    else
	    {
	        esc = false;
	    }
    }
}
