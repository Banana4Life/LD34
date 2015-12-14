using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class AI : MonoBehaviour
{

    public float initialDelay = 1f;
    public float stepDelay = 1f;
    public int actionsPerStep = 1;
    private List<Village> villages = new List<Village>();

    // Use this for initialization
    void GridReady(GameObject[,] grid)
    {
        foreach (var hex in grid)
        {
            var v = hex.GetComponentInChildren<Village>();
            if (v)
            {
                villages.Add(v);
            }
        }
        InvokeRepeating("AIStep", initialDelay, stepDelay);
    }

    private Dictionary<Faction, List<Village>> partitionedVillages()
    {
        var partitions = new Dictionary<Faction, List<Village>>();
        foreach (var village in this.villages)
        {
            List<Village> villages;
            if (!partitions.ContainsKey(village.faction))
            {
                villages = new List<Village>();
                partitions.Add(village.faction, villages);
            }
            else
            {
                villages = partitions[village.faction];
            }
            villages.Add(village);
        }

        return partitions;
    }

    void AIStep()
    {
        var villagesByFaction = partitionedVillages();
    }
}
