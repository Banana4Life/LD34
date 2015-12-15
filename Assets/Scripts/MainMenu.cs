using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool difficulty;
    private bool esc;

    void Start()
    {
        gameObject.transform.GetChild(6).gameObject.SetActive(false);
        gameObject.transform.GetChild(7).gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !difficulty)
        {
            gameObject.transform.GetChild(4).gameObject.SetActive(false);
            gameObject.transform.GetChild(5).gameObject.SetActive(false);
            gameObject.transform.GetChild(6).gameObject.SetActive(true);
            gameObject.transform.GetChild(7).gameObject.SetActive(true);
            difficulty = true;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Application.Quit();
        } 
        else if (Input.GetKey(KeyCode.Alpha1) && difficulty)
        {
            AI.easyMode = true;
            SceneManager.LoadScene("Main");
        }
        else if (Input.GetKey(KeyCode.Alpha2) && difficulty)
        {
            AI.easyMode = false;
            SceneManager.LoadScene("Main");
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if (!esc)
            {
                if (difficulty)
                {
                    gameObject.transform.GetChild(4).gameObject.SetActive(true);
                    gameObject.transform.GetChild(5).gameObject.SetActive(true);
                    gameObject.transform.GetChild(6).gameObject.SetActive(false);
                    gameObject.transform.GetChild(7).gameObject.SetActive(false);
                    difficulty = false;
                }
                else
                {
                    Application.Quit();
                }
            }
            esc = true;
        }
        else
        {
            esc = false;
        }
    }
}
