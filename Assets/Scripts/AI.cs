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
    public static double maxDistance;

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

        var distances = new List<double>();
        foreach (var a in villages)
        {
            foreach (var b in villages)
            {
                distances.Add(distance(a, b));
            }
        }
        maxDistance = distances.OrderByDescending(x => x).First();
        

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
        return partition(this.villages, v => v.faction);
    }

    private List<Action> possibleOffensive(IEnumerable<Village> sources, IEnumerable<Village> targets)
    {
        var actions = new List<Action>();
        foreach (var source in sources)
        {
            foreach (var target in targets)
            {
                actions.Add(new OffensiveAction(source, target));
            }
        }

        return actions;
    }

    private List<Action>  possibleDefensive(IEnumerable<Village> mine)
    {
        var actions = new List<Action>();
        foreach (var source in mine)
        {
            foreach (var target in mine)
            {
                actions.Add(new OffensiveAction(source, target));
            }
        }

        return actions;
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

        var actions = possibleOffensive(myVillages, targets).Union(possibleDefensive(myVillages));

        var scored = actions.Select(s => new ScoredAction(s, s.score(s.source, s.target)))
            .Where(s => s.score > 0)
            .OrderByDescending(s => s.score);

        var best = scored.Take(this.actionsPerStep);
        if (best.Any())
        {
            foreach (var action in best)
            {
                executeScenario(action.action);
            }
        }
    }

    void executeScenario(Action scenario)
    {
        scenario.source.releaseLegion(Vector3.one, scenario.target);
    }

    static double distance(Village a, Village b)
    {
        var ta = Tile.of(a.gameObject.transform.parent.gameObject);
        var tb = Tile.of(b.gameObject.transform.parent.gameObject);
        return PathFinder.defaultEstimation(ta)(tb);
    }

    static double force(Vector3 v)
    {
        return v.x + v.y + v.z;
    }

    protected class OffensiveAction : Action
    {

        public OffensiveAction(Village source, Village target) : base(source, target)
        {}

        public override double risk(Village origin, Village target)
        {
            var att = source.defForce;
            var def = target.defForce;
            var attForce = force(att);
            if (attForce < 15)
            {
                return 0;
            }
            return distance(source, target) + (source.size.unitCap - attForce) + (force(def) - attForce) + source.size.unitCap;
        }

        public override double profit(Village origin, Village target)
        {
            return target.size.unitCap + Math.Max(0, maxDistance - distance(origin, target));
        }
    }

    protected class ScoredAction
    {
        public readonly Action action;
        public readonly double score;

        public ScoredAction(Action action, double score)
        {
            this.action = action;
            this.score = score;
        }
    }

    protected class DefensiveAction : Action
    {
        public DefensiveAction(Village source, Village target) : base(source, target)
        {
        }

        public override double risk(Village origin, Village target)
        {
            return 1;
        }

        public override double profit(Village origin, Village target)
        {
            return 1;
        }
    }

    protected abstract class Action
    {
        public readonly Village source;
        public readonly Village target;

        protected Action(Village source, Village target)
        {
            this.source = source;
            this.target = target;
        }

        public abstract double risk(Village origin, Village target);
        public abstract double profit(Village origin, Village target);

        public double score(Village origin, Village target)
        {
            return profit(origin, target) / risk(origin, target);
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
