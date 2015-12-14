﻿using System.CodeDom;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Village : TileObject
{
    public static float percent = 50;

    public override bool canBePassed()
    {
        return false;
    }

    public GameObject villageTakenPrefab;
    public GameObject villageUIImagePrefab;
    public GameObject villageTextPrefab;
    public Faction faction;
    public Size size;
    public bool flipX;
    public bool flipY;
    public int angle;

    private GameObject villageText;
    private GameObject villageUIImage;

    public GameObject legUnit1;
    public GameObject legUnit2;
    public GameObject legUnit3;

    public Vector3 defForce = new Vector3(10, 0, 0);

    private GameObject factionObject;

    private SpriteRenderer getRenderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public void setSize(Size size)
    {
        this.size = size;
        if (faction == Faction.NEUTRAL)
        {
            size.production /= 2;
        }
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

        flipX = Random.value < 0.5;
        flipY = Random.value < 0.5;
        angle = Random.Range(0, 3) * 90;

        gameObject.transform.localEulerAngles = new Vector3(0, 0, angle);
        gameObject.transform.localScale = new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1);

        if (villageUIImage == null)
        {
            this.villageUIImage = Instantiate(villageUIImagePrefab);
            this.villageUIImage.transform.SetParent(gameObject.transform.parent);
            this.villageUIImage.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.villageUIImage.transform.localPosition = new Vector3(0, 2.5f, -1);
        }

        if (villageText == null)
        {
            this.villageText = Instantiate(villageTextPrefab);
            this.villageText.transform.SetParent(gameObject.transform.parent);
            this.villageText.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.villageText.transform.localPosition = new Vector3(0, 2.5f, -2);
        }

        updateText();
    }

    public int bonus = 4;

    public void fight(Vector3 atkForce, Faction faction)
    {
        if (this.faction == faction)
        {
            defForce += atkForce;
        }
        else
        {
            var defForce = new Vector3(this.defForce.x, this.defForce.y, this.defForce.z);
            float defMagnitude = defForce.x + defForce.y + defForce.z;
            float atkMagnitude = atkForce.x + atkForce.y + atkForce.z;

            if (defMagnitude != 0)
            {
                var defLoss = new Vector3(defForce.x / defMagnitude, defForce.y / defMagnitude, defForce.z / defMagnitude);
                var atkLoss = new Vector3(atkForce.x / atkMagnitude, atkForce.y / atkMagnitude, atkForce.z / atkMagnitude);

                // potential Attacker Damage 
                var atkDmg = new Vector3(atkForce.x + atkForce.y * bonus + atkForce.z,
                                         atkForce.x + atkForce.y + atkForce.z * bonus,
                                         atkForce.x * bonus + atkForce.y + atkForce.z);

                // potential Defender Damage 
                var defDmg = new Vector3(defForce.x + defForce.y * bonus + defForce.z,
                                         defForce.x + defForce.y + defForce.z * bonus,
                                         defForce.x * bonus + defForce.y + defForce.z);

                atkDmg.Scale(defLoss);
                defDmg.Scale(atkLoss);
                atkDmg /= bonus / 2;
                defDmg /= bonus / 2;


                //                Debug.Log("ATK(" + atkForce + ":" + atkMagnitude + ") " + atkDmg + " : DEF(" + defForce + ":" + defMagnitude + ") " + defDmg);
                defForce -= atkDmg;
                atkForce -= defDmg;
                this.defForce -= atkDmg;

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

                if (atkForce.x < 0)
                {
                    atkForce.x = 0;
                }
                if (atkForce.y < 0)
                {
                    atkForce.y = 0;
                }
                if (atkForce.z < 0)
                {
                    atkForce.z = 0;
                }

                if (defForce.sqrMagnitude == 0 && atkForce.sqrMagnitude != 0)
                {
                    Debug.Log("Attacker won: " + faction);
                    this.setFaction(faction);
                    this.defForce = atkForce;
                }
            }
        }
        updateText();
    }

    public void releaseLegion(Vector3 force, Tile start, Tile end)
    {
        var startVillage = start.getVillage();
        var endVillage = end.getVillage();

        var group = new GameObject("Legion Group");

        var atkForce = new Vector3((int)defForce.x, (int)defForce.y, (int)defForce.z);
        atkForce = atkForce * percent / 100;
        atkForce = new Vector3(Mathf.CeilToInt(atkForce.x), Mathf.CeilToInt(atkForce.y), Mathf.CeilToInt(atkForce.z));
        var amount = atkForce.x + atkForce.y + atkForce.z;

        Debug.Log("Release the Legion! Force:" + atkForce + "/" + defForce + " " + start.GameObject.transform.position + "->" + end.GameObject.transform.position);

        defForce -= atkForce;

        for (var i = 0; i < atkForce.x; i++)
        {
            PathWalker.walk(spawn(legUnit1, startVillage.gameObject, new Vector3(1, 0, 0), Faction.FRIENDLY, group, amount / 50), start, end);
        }
        for (var i = 0; i < atkForce.y; i++)
        {
            PathWalker.walk(spawn(legUnit2, startVillage.gameObject, new Vector3(0, 1, 0), Faction.FRIENDLY, group, amount / 50), start, end);
        }
        for (var i = 0; i < atkForce.z; i++)
        {
            PathWalker.walk(spawn(legUnit3, startVillage.gameObject, new Vector3(0, 0, 1), Faction.FRIENDLY, group, amount / 50), start, end);
        }
    }

    public GameObject spawn(GameObject type, GameObject at, Vector3 force, Faction faction, GameObject inHere, float spread)
    {
        var unit = Instantiate(type);
        Physics2D.IgnoreCollision(unit.GetComponent<Collider2D>(), at.GetComponent<Collider2D>(), true);
        unit.transform.position = at.transform.position + new Vector3((Random.value - 0.5f) * spread, (Random.value - 0.5f) * spread, 0);
        unit.transform.parent = inHere.transform;
        var unitForce = unit.GetComponent<Force>();
        unitForce.force = force;
        unitForce.faction = faction;
        return unit;
    }

    float delta;

    int unitType = Random.Range(0,3);
    float productionFactor = 20;

    void FixedUpdate()
    {
        delta += Time.fixedDeltaTime;
        if (delta > 0.5)
        {
            var units = defForce.x + defForce.y + defForce.z;
            delta = 0;
            switch(unitType)
            {
                case 0:
                    if (units <= size.unitCap)
                    {
                        defForce.x += size.production / productionFactor;
                    }
                    break;
                case 1:
                    if (units <= size.unitCap)
                    {
                        defForce.y += size.production / productionFactor;
                    }
                    break;
                case 2:
                    if (units <= size.unitCap)
                    {
                        defForce.z += size.production / productionFactor;
                    }
                    break;
            }
        }
        updateText();
    }

    private void updateText()
    {
        villageText.GetComponent<Text>().text = (int)defForce.x + "/" + (int)defForce.y + "/" + (int)defForce.z;
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

    public float production;
    public int unitCap;

    private static List<Size> SIZES;

    private Size()
    {
        sprite = sprites[index];
        radius = MIN_RADIUS + (++index) * RADIUS_STEP_SCALE;
        production = index * index * 2;
        unitCap = index * 50;

        if (SIZES == null)
        {
            SIZES = new List<Size>();
        }
        SIZES.Add(this);
    }
}