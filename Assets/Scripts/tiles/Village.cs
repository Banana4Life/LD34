using System.CodeDom;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Village : TileObject
{
    public static float percent = 70;

    public override bool canBePassed()
    {
        return false;
    }

    public GameObject villageTakenPrefab;
    public GameObject villageUiBackgroundPrefab;
    public GameObject villageUiOverlayPrefab;
    public GameObject villageTextPrefab;
    public Sprite villageUiNeutral;
    public Sprite villageUiAlien;
    public Sprite villageUiLegionnaire;
    public Sprite villageUiOverlayNeutral;
    public Sprite villageUiOverlayAlien;
    public Sprite villageUiOverlayLegionnaire;
    public Faction faction;
    public Size size;
    public bool flipX;
    public bool flipY;
    public int angle;

    private List<GameObject> villageTexts;
    private GameObject villageUIBackground;
    private GameObject villageUIOverlay;

    public GameObject legUnit1;
    public GameObject legUnit2;
    public GameObject legUnit3;

    public Vector3 defForce = new Vector3(10, 0, 0);
    private readonly int unitType = Random.Range(0, 3);

    private GameObject factionObject;

    public AudioClip[] releaseSounds;
    public float releaseSoundsVol;

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
        angle = Random.Range(0, 3)*90;

        gameObject.transform.localEulerAngles = new Vector3(0, 0, angle);
        gameObject.transform.localScale = new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1);

        if (villageUIBackground == null)
        {
            this.villageUIBackground = Instantiate(villageUiBackgroundPrefab);
            this.villageUIBackground.transform.SetParent(gameObject.transform.parent);
            this.villageUIBackground.transform.localPosition = new Vector3(0, 2.5f, -1);
        }
        if (villageUIOverlay == null)
        {
            this.villageUIOverlay = Instantiate(villageUiOverlayPrefab);
            this.villageUIOverlay.transform.SetParent(villageUIBackground.transform);
            this.villageUIOverlay.transform.localPosition = new Vector3(0, 0, -1);
        }
        updateUI();

        if (villageTexts == null)
        {
            villageTexts = new List<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                villageTexts.Add(Instantiate(villageTextPrefab));
                villageTexts[i].transform.SetParent(villageUIBackground.transform);
                villageTexts[i].transform.localPosition = new Vector3(-13 + 16.5f * i, 0, -1);
                if (i == unitType)
                {
                    villageTexts[i].GetComponent<Text>().color = new Color(251f / 255f, 242f / 255f, 54f / 255f);
                }
            }
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
                var defLoss = new Vector3(defForce.x/defMagnitude, defForce.y/defMagnitude, defForce.z/defMagnitude);
                var atkLoss = new Vector3(atkForce.x/atkMagnitude, atkForce.y/atkMagnitude, atkForce.z/atkMagnitude);

                // potential Attacker Damage 
                var atkDmg = new Vector3(atkForce.x + atkForce.y*bonus + atkForce.z,
                    atkForce.x + atkForce.y + atkForce.z*bonus,
                    atkForce.x*bonus + atkForce.y + atkForce.z);

                // potential Defender Damage 
                var defDmg = new Vector3(defForce.x + defForce.y*bonus + defForce.z,
                    defForce.x + defForce.y + defForce.z*bonus,
                    defForce.x*bonus + defForce.y + defForce.z);

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
                    this.defForce.x = 0;
                }
                if (defForce.y < 0)
                {
                    defForce.y = 0;
                    this.defForce.y = 0;
                }
                if (defForce.z < 0)
                {
                    defForce.z = 0;
                    this.defForce.z = 0;
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
                    updateUI(); // Update Background of UI to new faction
                    this.defForce = atkForce;
                }
            }
        }
        updateText();
    }

    public void releaseLegion(Vector3 force, Village target)
    {
        releaseLegion(force, Tile.of(gameObject.transform.parent.gameObject), Tile.of(target.gameObject.transform.parent.gameObject));
    }

    public void releaseLegion(Vector3 force, Tile start, Tile end)
    {
        if (start == end)
        {
            Debug.Log("Tried to release a legion to itself:" + start);
            return;
        }

        var startVillage = start.getVillage();
        var endVillage = end.getVillage();

        var atkForce = new Vector3((int) defForce.x, (int) defForce.y, (int) defForce.z);
        atkForce = atkForce*percent/100;
        atkForce = new Vector3(Mathf.CeilToInt(atkForce.x), Mathf.CeilToInt(atkForce.y), Mathf.CeilToInt(atkForce.z));
        var amount = atkForce.x + atkForce.y + atkForce.z;

        Debug.Log("Release the Legion! Force:" + atkForce + "/" + defForce + " " + start.GameObject.transform.position +
                  "->" + end.GameObject.transform.position);

        defForce -= atkForce;

        var group = new GameObject("Legion Group");
        group.tag = AttackingLegion.TAG;
        var attackingLegion = group.AddComponent<AttackingLegion>();
        attackingLegion.origin = startVillage;
        attackingLegion.destination = endVillage;
        attackingLegion.faction = faction;
        attackingLegion.force = atkForce;

        for (var i = 0; i < atkForce.x; i++)
        {
            PathWalker.walk(
                spawn(legUnit1, startVillage.gameObject, new Vector3(1, 0, 0), this.faction, group, amount/50),
                start, end);
        }
        for (var i = 0; i < atkForce.y; i++)
        {
            PathWalker.walk(
                spawn(legUnit2, startVillage.gameObject, new Vector3(0, 1, 0), this.faction, group, amount/50),
                start, end);
        }
        for (var i = 0; i < atkForce.z; i++)
        {
            PathWalker.walk(
                spawn(legUnit3, startVillage.gameObject, new Vector3(0, 0, 1), this.faction, group, amount/50),
                start, end);
        }
    }

    public GameObject spawn(GameObject type, GameObject at, Vector3 force, Faction faction, GameObject inHere,
        float spread)
    {
        var unit = Instantiate(type);
        Physics2D.IgnoreCollision(unit.GetComponent<Collider2D>(), at.GetComponent<Collider2D>(), true);
        unit.transform.position = at.transform.position +
                                  new Vector3((Random.value - 0.5f)*spread, (Random.value - 0.5f)*spread, 0);
        unit.transform.parent = inHere.transform;
        var unitForce = unit.GetComponent<Force>();
        unitForce.force = force;
        unitForce.faction = faction;
        return unit;
    }

    float delta;
    float productionFactor = 20;

    void FixedUpdate()
    {
        delta += Time.fixedDeltaTime;
        if (delta > 0.5)
        {
            var units = defForce.x + defForce.y + defForce.z;
            delta = 0;
            switch (unitType)
            {
                case 0:
                    if (units <= size.unitCap)
                    {
                        defForce.x += size.production/productionFactor;
                    }
                    break;
                case 1:
                    if (units <= size.unitCap)
                    {
                        defForce.y += size.production/productionFactor;
                    }
                    break;
                case 2:
                    if (units <= size.unitCap)
                    {
                        defForce.z += size.production/productionFactor;
                    }
                    break;
            }
        }
        updateText();
    }

    private void updateText()
    {
        for (int i = 0; i < 3; i++)
        {
            villageTexts[i].GetComponent<Text>().text = (int) defForce[i] + "";
        }
    }

    private void updateUI()
    {
        if (faction == Faction.NEUTRAL)
        {
            villageUIOverlay.GetComponent<Image>().sprite = villageUiOverlayNeutral;
            villageUIBackground.GetComponent<Image>().sprite = villageUiNeutral;
        }
        else if (faction == Faction.ENEMY)
        {
            villageUIOverlay.GetComponent<Image>().sprite = villageUiOverlayAlien;
            villageUIBackground.GetComponent<Image>().sprite = villageUiAlien;
        }
        else
        {
            villageUIOverlay.GetComponent<Image>().sprite = villageUiOverlayLegionnaire;
            villageUIBackground.GetComponent<Image>().sprite = villageUiLegionnaire;
        }
    }
}

public class Faction
{
    public static readonly Faction FRIENDLY = new Faction("Legionare", Color.green);
    public static readonly Faction NEUTRAL = new Faction("Nobody", Color.yellow);
    public static readonly Faction ENEMY = new Faction("Aliens", Color.red);

    public readonly string name;
    public readonly Color color;

    private Faction(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }

    public override string ToString()
    {
        return this.name;
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
