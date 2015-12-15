using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void GridReady()
    {
        // do nothing
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            SceneManager.LoadScene("Main");
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
