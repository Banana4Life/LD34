using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    public static GameObject uiOverlay;
    public static GameObject scrollUi;

    void Start ()
    {
        uiOverlay = gameObject;
        scrollUi = gameObject.transform.GetChild(0).gameObject;
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
            uiOverlay.transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}
