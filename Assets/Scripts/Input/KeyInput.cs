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
	    else if (Input.GetKey(KeyCode.E))
	    {
	        if (UIOverlay.paused)
	        {
                Application.Quit();
            }
	    }
        else
	    {
	        esc = false;
	    }
    }
}
