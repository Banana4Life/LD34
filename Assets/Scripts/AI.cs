using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;

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

    private List<AttackScenario> possibleScenarios(List<Village> sources, List<Village> targets)
    {
        var scenarios = new List<AttackScenario>();
        foreach (var source in sources)
        {
            foreach (var target in targets)
            {
                scenarios.Add(new AttackScenario(source, target));
            }
        }

        return scenarios;
    }

    void AIStep()
    {
        var villagesByFaction = partitionedVillages();

        var myVillages = villagesByFaction[Faction.ENEMY];
        var hisVillages = villagesByFaction[Faction.FRIENDLY];
        var openVillages = villagesByFaction[Faction.NEUTRAL];
        var targets = hisVillages.Union(openVillages).ToList();

        var scenarios = possibleScenarios(myVillages, targets);


        var max = scenarios.Select(s => new ScoredScenario(s, score(s.source, s.target))).OrderByDescending(s => s.score).First();
        Debug.Log(max.score);
        executeScenario(max.scenario);
    }

    void executeScenario(AttackScenario scenario)
    {
        scenario.source.releaseLegion(Vector3.one, scenario.target);
    }

    double distance(Village a, Village b)
    {
        var ta = Tile.of(a.gameObject.transform.parent.gameObject);
        var tb = Tile.of(b.gameObject.transform.parent.gameObject);
        return PathFinder.defaultEstimation(ta)(tb);
    }

    double force(Vector3 v)
    {
        return v.x + v.y + v.z;
    }

    double score(Village source, Village target)
    {
        var att = source.defForce;
        var def = target.defForce;
        return 1d/distance(source, target) + force(def) - force(att);
    }

    protected class AttackScenario
    {
        public readonly Village source;
        public readonly Village target;

        public AttackScenario(Village source, Village target)
        {
            this.source = source;
            this.target = target;
        }
    }

    protected class ScoredScenario
    {
        public readonly AttackScenario scenario;
        public readonly double score;

        public ScoredScenario(AttackScenario scenario, double score)
        {
            this.scenario = scenario;
            this.score = score;
        }
    }
}
