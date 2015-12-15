using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    public static GameObject uiOverlay;
    public static GameObject scrollUi;

    public static bool paused = false;

    void Start ()
    {
        uiOverlay = gameObject;
        scrollUi = uiOverlay.transform.GetChild(0).gameObject;
        for (int i = 0; i < uiOverlay.transform.childCount; i++)
        {
            uiOverlay.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void factionDefeated(Faction f)
    {
        if (f == Faction.ENEMY)
        {
            uiOverlay.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (f == Faction.FRIENDLY)
        {
            var youwon = uiOverlay.transform.GetChild(2).gameObject;
            youwon.SetActive(true);
            youwon.GetComponent<AudioSource>().Play();
        }
    }

    public static void pauseUnpause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        uiOverlay.transform.GetChild(3).gameObject.SetActive(paused);
    }
}
