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

    public void FactionDefeated(Faction f)
    {
        if (f == Faction.ENEMY)
        {
            var youwon = uiOverlay.transform.GetChild(1).gameObject;
            youwon.SetActive(true);
            Camera.main.GetComponent<AudioSource>().mute = true;
            youwon.GetComponent<AudioSource>().Play();
            uiOverlay.transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (f == Faction.FRIENDLY)
        {
            var youlose = uiOverlay.transform.GetChild(2).gameObject;
            youlose.SetActive(true);

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

        var friendlyPartition = villages.GetOrElse(Faction.FRIENDLY, new List<Village>());
        var neutralPartition = villages.GetOrElse(Faction.NEUTRAL, new List<Village>());
        var enemyPartition = villages.GetOrElse(Faction.ENEMY, new List<Village>());
        float villageCount = friendlyPartition.Count + neutralPartition.Count +
                           enemyPartition.Count;
        float friendlyCount = friendlyPartition.Count/villageCount*500;
        float neutralCount = neutralPartition.Count/villageCount*500;
        float enemyCount = enemyPartition.Count/villageCount*500;

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