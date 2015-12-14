using UnityEngine;
using System.Collections;

public class River : TileObject
{
    public double cost = 5d;

    public override double passingCost()
    {
        return cost;
    }
}
