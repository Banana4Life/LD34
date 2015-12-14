using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : MonoBehaviour
{
    public float initialDelay = 1f;
    public float stepDelay = 1f;
    public int actionsPerStep = 1;
    private List<Village> villages = new List<Village>();

    private static readonly string STEP_METHOD = "AIStep";
    private static readonly string DEFEATED_MESSAGE = "FactionDefeated";

    void Start()
    {
        Application.runInBackground = true;
    }

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
        InvokeRepeating(STEP_METHOD, initialDelay, stepDelay);
    }

    private static IDictionary<K, IList<V>> partition<K, V>(IEnumerable<V> input, Func<V, K> key)
    {
        IDictionary<K, IList<V>> partitions = new Dictionary<K, IList<V>>();
        foreach (var v in input)
        {
            IList<V> partition;
            var k = key(v);
            if (!partitions.ContainsKey(k))
            {
                partition = new List<V>();
                partitions.Add(k, partition);
            }
            else
            {
                partition = partitions[k];
            }
            partition.Add(v);
        }

        return partitions;
    }

    private IDictionary<Faction, IList<Village>> partitionedVillages()
    {
        return partition(this.villages, v => {

            if (v.faction == null)
            {
                Debug.Log(v + " " + Tile.of(v.gameObject.transform.parent.gameObject) + " " + v.size);
            }
            return v.faction;

            });
    }

    private List<AttackScenario> possibleScenarios(IEnumerable<Village> sources, IEnumerable<Village> targets)
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

    void finished(Faction defeated)
    {
        SendMessage(DEFEATED_MESSAGE, defeated);
        CancelInvoke(STEP_METHOD);
        Destroy(this);
    }

    void AIStep()
    {
        var legionsByFaction = partition(GameObject.FindGameObjectsWithTag(AttackingLegion.TAG).Select(g => g.GetComponent<AttackingLegion>()), a => a.faction);
        var incomingAttacks = legionsByFaction.GetOrElse(Faction.FRIENDLY, new List<AttackingLegion>());
        var outgoingAttacks = legionsByFaction.GetOrElse(Faction.ENEMY, new List<AttackingLegion>());

        if (outgoingAttacks.Count > 2)
        {
            return;
        }

        var villagesByFaction = partitionedVillages();

        var myVillages = villagesByFaction.GetOrElse(Faction.ENEMY, new List<Village>());
        if (myVillages.Count == 0)
        {
            finished(Faction.ENEMY);
            return;
        }

        var hisVillages = villagesByFaction.GetOrElse(Faction.FRIENDLY, new List<Village>());
        if (hisVillages.Count == 0)
        {
            finished(Faction.FRIENDLY);
            return;
        }

        var openVillages = villagesByFaction.GetOrElse(Faction.NEUTRAL, new List<Village>());
        var targets = hisVillages.Union(openVillages).ToList();

        var scenarios = possibleScenarios(myVillages, targets);

        var leastRisky = scenarios.Select(s => new ScoredScenario(s, risk(s.source, s.target)))
            .Where(s => s.score >= 0)
            .OrderBy(s => s.score);

        if (leastRisky.Any())
        {
            executeScenario(leastRisky.First().scenario);
        }
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

    double risk(Village source, Village target)
    {
        var att = source.defForce;
        var def = target.defForce;
        var attForce = force(att);
        if (attForce < 15)
        {
            return -1;
        }
        return distance(source, target) + (source.size.unitCap - attForce) + (force(def) - attForce) + source.size.unitCap;
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

public static class MapHelper
{
    public static V GetOrElse<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue)
    {
        V value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }
}
