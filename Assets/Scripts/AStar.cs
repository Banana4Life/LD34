using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

//struct holding x and y coordinates
public struct Point
{
    public int X, Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point add(Point p)
    {
        return new Point(X + p.X, Y + p.Y);
    }
}

//abstract class implemented by Tile class
public abstract class GridObject
{
    public Point Location;
    public int X { get { return Location.X; } }
    public int Y { get { return Location.Y; } }

    public GridObject(Point location)
    {
        Location = location;
    }

    public GridObject(int x, int y) : this(new Point(x, y)) { }

    public override string ToString()
    {
        return string.Format("({0}, {1})", X, Y);
    }
}

//interface that should be implemented by grid nodes used in E. Lippert's generic path finding implementation
public interface IHasNeighbours<N>
{
    IEnumerable<N> FreeNeighbours { get; }
}

public class Tile : GridObject, IHasNeighbours<Tile>
{
    public readonly GameObject GameObject;

    public Tile(int x, int y, GameObject gameObject) : base(x, y)
    {
        this.GameObject = gameObject;
    }

    public bool canBePassed()
    {
        //return this.GameObject.GetComponent<HasVillage>() != null;
        //return this.GameObject.GetComponentInChildren<Village>() != null;
        return true;
    }

    public IEnumerable<Tile> Neighbours { get; set; }
    public IEnumerable<Tile> FreeNeighbours
    {
        get { return Neighbours.Where(o => o.canBePassed()); }
    }
    public static List<Point> NeighbourShift
    {
        get
        {
            return new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, -1),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, 1),
                };
        }
    }

    public void FindNeighbours(Dictionary<Point, Tile> board)
    {
        List<Tile> neighbours = new List<Tile>();

        foreach (Point point in NeighbourShift)
        {
            var neighbourPoint = Location.add(point);

            if (board.ContainsKey(neighbourPoint))
            {
                neighbours.Add(board[neighbourPoint]);
            }
        }

        Neighbours = neighbours;
    }
}

public class Path<Node> : IEnumerable<Node>
{
    public Node LastStep { get; private set; }
    public Path<Node> PreviousSteps { get; private set; }
    public double TotalCost { get; private set; }
    private Path(Node lastStep, Path<Node> previousSteps, double totalCost)
    {
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }
    public Path(Node start) : this(start, null, 0) { }
    public Path<Node> AddStep(Node step, double stepCost)
    {
        return new Path<Node>(step, this, TotalCost + stepCost);
    }
    public IEnumerator<Node> GetEnumerator()
    {
        for (Path<Node> p = this; p != null; p = p.PreviousSteps)
            yield return p.LastStep;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

class PriorityQueue<P, V>
{
    private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
    public void Enqueue(P priority, V value)
    {
        Queue<V> q;
        if (!list.TryGetValue(priority, out q))
        {
            q = new Queue<V>();
            list.Add(priority, q);
        }
        q.Enqueue(value);
    }
    public V Dequeue()
    {
        // will throw if there isn’t any first element!
        var pair = list.First();
        var v = pair.Value.Dequeue();
        if (pair.Value.Count == 0) // nothing left of the top priority.
            list.Remove(pair.Key);
        return v;
    }
    public bool IsEmpty
    {
        get { return !list.Any(); }
    }
}

public static class PathFinder
{
    //distance f-ion should return distance between two adjacent nodes
    //estimate should return distance between any node and destination node
    public static Path<Node> FindPath<Node>(Node start, Node destination,
        Func<Node, Node, double> distance, Func<Node, double> estimate) where Node : IHasNeighbours<Node>
    {
        //set of already checked nodes
        var closed = new HashSet<Node>();
        //queued nodes in open set
        var queue = new PriorityQueue<double, Path<Node>>();
        queue.Enqueue(0, new Path<Node>(start));

        while (!queue.IsEmpty)
        {
            var path = queue.Dequeue();

            if (closed.Contains(path.LastStep))
                continue;
            if (path.LastStep.Equals(destination))
                return path;

            closed.Add(path.LastStep);

            foreach (Node n in path.LastStep.FreeNeighbours)
            {
                double d = distance(path.LastStep, n);
                //new step added without modifying current path
                var newPath = path.AddStep(n, d);
                queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
            }
        }

        return null;
    }
}