using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tile : GridObject, IHasNeighbours<Tile>
{
    public readonly GameObject GameObject;

    public Tile(int x, int y, GameObject gameObject) : base(x, y)
    {
        this.GameObject = gameObject;
    }

    public bool canBePassed()
    {
        var obj = this.GameObject.GetComponentInChildren<TileObject>();
        if (obj == null)
        {
            return true;
        }
        return obj.canBePassed();
    }

    public IEnumerable<Tile> Neighbours { get; set; }
    public IEnumerable<Tile> FreeNeighbours(Tile destination = null)
    {
        return Neighbours.Where(t => t.canBePassed() || t == destination);
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

    public static Tile of(GameObject obj)
    {
        var th = obj.GetComponent<TileHolder>();
        if (th)
        {
            return th.tile;
        }
        return null;
    }

    public T fromChildren<T>()
    {
        return GameObject.GetComponentInChildren<T>();
    }
}

