using UnityEngine;
using System.Collections;

public class ScrollUI : MonoBehaviour {
    void Start ()
    {
        HexInput.scrollUi = gameObject;
        CameraScroll.scrollUi = gameObject;
        gameObject.SetActive(false);
    }
}
