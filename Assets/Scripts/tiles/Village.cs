using System;
using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;

public class Village : TileObject
{
    private static readonly Random RANDOM = new Random();

    public override bool canBePassed()
    {
        return false;
    }

    public GameObject villageTakenPrefab;
    public Faction faction;
    public Size size;
    public bool flipX;
    public bool flipY;
    public float angle;

    public Vector3 defForce = new Vector3(10, 0, 0);

    private GameObject factionObject;

    private SpriteRenderer getRenderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public void setSize(Size size)
    {
        this.size = size;
        getRenderer().sprite = size.sprite;
    }

    public void setFaction(Faction faction)
    {
        if (!this.factionObject)
        {
            this.factionObject = Instantiate(villageTakenPrefab);
            this.factionObject.transform.parent = gameObject.transform;
            this.factionObject.transform.localPosition = new Vector3(0, 0, -1);
        }
        var villageTakenScript = this.factionObject.GetComponent<Village_Taken>();
        if (villageTakenScript)
        {
            villageTakenScript.Adapt(this.size, faction);
        }
        this.faction = faction;
    }

    void Start()
    {

        if (this.size == null)
        {
            setSize(Size.CAMP);
        }
        if (this.faction == null)
        {
            setFaction(Faction.NEUTRAL);
        }

        flipX = RANDOM.Next(2) != 0;
        flipY = RANDOM.Next(2) != 0;
        angle = RANDOM.Next(4) * 90;

        gameObject.transform.localEulerAngles = new Vector3(0, 0, angle);
        gameObject.transform.localScale = new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1);
    }

    public int bonus = 4;

    void fight(Vector3 atkForce, Faction faction)
    {
        if (this.faction == faction)
        {
            defForce += atkForce;
        }
        else
        {
            var defLossX = 0f;
            var defLossY = 0f;
            var defLossZ = 0f;
            var atkLossX = 0f;
            var atkLossY = 0f;
            var atkLossZ = 0f;

            float defMagnitude = defForce.x + defForce.y + defForce.z;
            float atkMagnitude = atkForce.x + atkForce.y + atkForce.z;
            // Attacker...
            // Attack X (Melee)
            defLossX += defForce.x / defMagnitude * atkForce.x * 1 * 1;
            defLossY += defForce.x / defMagnitude * atkForce.y * 1 * 1;
            defLossZ += defForce.x / defMagnitude * atkForce.z * 1 * bonus;
            // Attack Y (Ranged)
            defLossX += defForce.y / defMagnitude * atkForce.x * 1 * bonus;
            defLossY += defForce.y / defMagnitude * atkForce.y * 1 * 1;
            defLossZ += defForce.y / defMagnitude * atkForce.z * 1 * 1;
            // Attack Z (Mounted)
            defLossX += defForce.z / defMagnitude * atkForce.x * 1 * 1;
            defLossY += defForce.z / defMagnitude * atkForce.y * 1 * bonus;
            defLossZ += defForce.z / defMagnitude * atkForce.z * 1 * 1;
            // Defender...
            // Defend X (Melee)
            atkLossX += atkForce.x / atkMagnitude * defForce.x * 1 * 1;
            atkLossY += atkForce.x / atkMagnitude * defForce.y * 1 * 1;
            atkLossZ += atkForce.x / atkMagnitude * defForce.z * 1 * bonus;
            // Defend Y (Ranged)
            atkLossX += atkForce.y / atkMagnitude * defForce.x * 1 * bonus;
            atkLossY += atkForce.y / atkMagnitude * defForce.y * 1 * 1;
            atkLossZ += atkForce.y / atkMagnitude * defForce.z * 1 * 1;
            // Defend Z (Mounted)
            atkLossX += atkForce.z / atkMagnitude * defForce.x * 1 * 1;
            atkLossY += atkForce.z / atkMagnitude * defForce.y * 1 * bonus;
            atkLossZ += atkForce.z / atkMagnitude * defForce.z * 1 * 1;

            defForce -= new Vector3(defLossX, defLossY, defLossZ);
            atkForce -= new Vector3(atkLossX, atkLossY, atkLossZ);

            if (defForce.x < 0)
            {
                defForce.x = 0;
            }
            if (defForce.y < 0)
            {
                defForce.y = 0;
            }
            if (defForce.z < 0)
            {
                defForce.z = 0;
            }

            if (defForce.sqrMagnitude == 0)
            {
                Debug.Log("Attacker won: " + faction);
                this.faction = faction;
                defForce = atkForce;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
        collision.collider.name);
    }
}

public class Faction
{
    public static readonly Faction FRIENDLY = new Faction(Color.green);
    public static readonly Faction NEUTRAL = new Faction(Color.yellow);
    public static readonly Faction ENEMY = new Faction(Color.red);

    public readonly Color color;

    private Faction(Color color)
    {
        this.color = color;
    }
}

public class Size
{
    public static readonly float MIN_RADIUS = 0.3f;
    public static readonly float RADIUS_STEP_SCALE = 0.12f;

    public static Sprite[] sprites = Resources.LoadAll<Sprite>("villages");
    public static int index = 0;

    public static readonly Size CAMP = new Size();
    public static readonly Size VILLAGE = new Size();
    public static readonly Size CASTLE = new Size();

    public readonly float radius;
    public readonly Sprite sprite;

    private static List<Size> SIZES;

    private Size()
    {
        sprite = sprites[index];
        radius = MIN_RADIUS + (++index) * RADIUS_STEP_SCALE;

        if (SIZES == null)
        {
            SIZES = new List<Size>();
        }
        SIZES.Add(this);
    }
}
