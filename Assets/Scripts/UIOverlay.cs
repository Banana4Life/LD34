using System.Collections.Generic;
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
        for (int i = 0; i < uiOverlay.transform.childCount - 1; i++)
        {
            uiOverlay.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void factionDefeated(Faction f)
    {
        if (f == Faction.ENEMY)
        {
            uiOverlay.transform.GetChild(1).gameObject.SetActive(true);
            uiOverlay.transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (f == Faction.FRIENDLY)
        {
            var youwon = uiOverlay.transform.GetChild(2).gameObject;
            youwon.SetActive(true);
            youwon.GetComponent<AudioSource>().Play();

            uiOverlay.transform.GetChild(3).gameObject.SetActive(true);
            uiOverlay.transform.GetChild(4).gameObject.SetActive(true);
        }
    }

    public static void pauseUnpause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        uiOverlay.transform.GetChild(5).gameObject.SetActive(paused);
    }

    void Update()
    {
        IDictionary<Faction, IList<Village>> villages = HexGrid.villagesByFaction();

        float villageCount = villages[Faction.FRIENDLY].Count + villages[Faction.NEUTRAL].Count +
                           villages[Faction.ENEMY].Count;
        float friendlyCount = villages[Faction.FRIENDLY].Count/villageCount*500;
        float neutralCount = villages[Faction.NEUTRAL].Count/villageCount*500;
        float enemyCount = villages[Faction.ENEMY].Count/villageCount*500;

        Transform transform = uiOverlay.transform.GetChild(6);

        RectTransform barPart = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        barPart.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, friendlyCount);

        barPart = transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        barPart.anchoredPosition = new Vector3(friendlyCount, 0, 0);
        barPart.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, neutralCount);

        barPart = transform.GetChild(2).gameObject.GetComponent<RectTransform>();
        barPart.anchoredPosition = new Vector3(friendlyCount + neutralCount, 0, 0);
        barPart.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, enemyCount);
    }
}