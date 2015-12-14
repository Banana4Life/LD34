using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class AttackingLegion : MonoBehaviour
{
    public static readonly string TAG = "legion_attack";
    public Village origin;
    public Village destination;
    public Vector3 force;
    public Faction faction;
}
