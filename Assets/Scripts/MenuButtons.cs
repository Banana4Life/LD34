using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {
    public enum Type
    {
        START,
        CREDITS,
        QUIT,
        NONE
    }

    public Type type;

    void OnMouseDown()
    {
        switch (type)
        {
            case Type.START:
                SceneManager.LoadScene("Main");
                break;
            case Type.CREDITS:
                break;
            case Type.QUIT:
                Application.Quit();
                break;
            case Type.NONE:
                break;
        }
    }
}
