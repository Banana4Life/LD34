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
    public int concurrency = 3;
    public bool easyMode = false;

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
        var distances = new List<double>();
        var villages = HexGrid.villages.Select(g => g.GetComponentInChildren<Village>());
        foreach (var a in villages)
        {
            foreach (var b in villages)
            {
                if (a != b)
                {
                    distances.Add(distance(a, b));
                }
            }
        }
        maxDistance = distances.OrderByDescending(x => x).First();
        

        InvokeRepeating(STEP_METHOD, initialDelay, stepDelay);
    }

    private List<Action> possibleOffensive(IEnumerable<Village> sources, IEnumerable<Village> targets)
    {
        var actions = new List<Action>();
        foreach (var source in sources)
        {
            foreach (var target in targets)
            {
                actions.Add(new OffensiveAction(source, target, this.easyMode));
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
                if (source != target)
                {
                    actions.Add(new DefensiveAction(source, target));
                }
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
        var legionsByFaction = HexGrid.partition(GameObject.FindGameObjectsWithTag(AttackingLegion.TAG).Select(g => g.GetComponent<AttackingLegion>()), a => a.faction);
        var incomingAttacks = legionsByFaction.GetOrElse(Faction.FRIENDLY, new List<AttackingLegion>());
        var outgoingAttacks = legionsByFaction.GetOrElse(Faction.ENEMY, new List<AttackingLegion>());

        if (outgoingAttacks.Count >= this.concurrency)
        {
            return;
        }

        var villagesByFaction = HexGrid.villagesByFaction();

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
            foreach (var scoredAction in best)
            {
                var action = scoredAction.action;
                Debug.Log(action.GetType().Name + " with " + scoredAction.score + " for " + Tile.of(action.target.gameObject.transform.parent.gameObject));
                executeScenario(action);
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
        private readonly bool easyMode;

        public OffensiveAction(Village source, Village target, bool easyMode) : base(source, target)
        {
            this.easyMode = easyMode;
        }

        public override double risk(Village origin, Village target)
        {
            var malus = this.easyMode ? 2d : 1d;
            if (target.unitType == origin.unitType)
            {
                malus = 2d;
            }
            else if (origin.unitType != ((target.unitType + 1) % 3))
            {
                malus = this.easyMode ? 4d : 16d;
            }
            var att = source.defForce;
            var def = target.defForce;
            var attForce = force(att);
            if (attForce < 15)
            {
                return 0;
            }
            if (target.faction == Faction.NEUTRAL)
            {
                malus *= .5;
            }
            var r = distance(source, target) + (source.size.unitCap - attForce) + (force(def) - attForce) + source.size.unitCap;
            return r * malus;
        }

        public override double profit(Village origin, Village target)
        {
            return target.size.production * .5d + Math.Max(0, maxDistance - distance(origin, target));
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
            return 0;
        }

        public override double profit(Village origin, Village target)
        {
            return 0;
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
            var r = risk(origin, target);
            if (r <= 0)
            {
                return 0;
            }
            var p = profit(origin, target);
            var s = p/r;
            return s;
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
