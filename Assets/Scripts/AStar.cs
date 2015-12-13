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

    public override int GetHashCode()
    {
        return X + 31 * Y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        var p = (Point)obj;
        return X == p.X && Y == p.Y;
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

    public override int GetHashCode()
    {
        return Location.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return ((GridObject) obj).Location.Equals(Location);
    }
}

//interface that should be implemented by grid nodes used in E. Lippert's generic path finding implementation
public interface IHasNeighbours<N>
{
    IEnumerable<N> FreeNeighbours(N desitination);
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
        var alreadyChecked = new HashSet<Node>();
        var queue = new PriorityQueue<double, Path<Node>>();
        queue.Enqueue(0, new Path<Node>(start));

        while (!queue.IsEmpty)
        {
            var path = queue.Dequeue();

            if (alreadyChecked.Contains(path.LastStep))
                continue;
            if (path.LastStep.Equals(destination))
                return path;

            alreadyChecked.Add(path.LastStep);

            foreach (Node n in path.LastStep.FreeNeighbours(destination))
            {
                double d = distance(path.LastStep, n);
                //new step added without modifying current path
                var newPath = path.AddStep(n, d);
                queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
            }
        }

        return null;
    }

    public static Func<Node, double> defaultEstimation<Node>(Node destination) where Node : GridObject
    {
        return (tile) =>
        {
            float deltaX = Mathf.Abs(destination.X - tile.X);
            float deltaY = Mathf.Abs(destination.Y - tile.Y);
            var z1 = -(tile.X + tile.Y);
            var z2 = -(destination.X + destination.Y);
            float deltaZ = Mathf.Abs(z2 - z1);

            return Mathf.Max(deltaX, deltaY, deltaZ);
        };
    }

    public static Path<Node> FindPath<Node>(Node start, Node end) where Node : GridObject, IHasNeighbours<Node>
    {
        return FindPath(start, end, (n, n2) => 1, defaultEstimation(end));
    }
}