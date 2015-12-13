using System;
using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;

public class Village : MonoBehaviour
{
    public static readonly float MIN_RADIUS = 0.3f;
    public static readonly float RADIUS_STEP_SCALE = 0.12f;
    private static readonly Random RANDOM = new Random();

    // Village sprites
    public static Sprite[] camp = Resources.LoadAll<Sprite>("villages");

    public class Faction
    {
        public static readonly Faction FRIENDLY = new Faction(Color.green);
        public static readonly Faction NEUTRAL = new Faction(Color.yellow);
        public static readonly Faction ENEMY = new Faction(Color.red);

        public readonly Color color;

        private Faction(Color color) {
            this.color = color;
        }
    }

    public class Size
    {
        public static int index = 0;

        public static readonly Size CAMP = new Size();
        public static readonly Size VILLAGE = new Size();
        public static readonly Size CASTLE = new Size();

        public readonly float radius;
        public readonly Sprite sprite;

        private static List<Size> SIZES;

        private Size()
        {
            sprite = camp[index];
            radius = MIN_RADIUS + (1 + index++) * RADIUS_STEP_SCALE;

            if (SIZES == null)
            {
                SIZES = new List<Size>();
            }
            SIZES.Add(this);
        }

        public static Size random()
        {
            return SIZES[RANDOM.Next(index)];
        }
    }

    public GameObject villageTakenPrefab;
    private SpriteRenderer renderer;
    public Faction faction = Faction.NEUTRAL;
    public Size size;
    public bool flipX;
    public bool flipY;
    public float angle;

    public Vector3 defendingUnits;
    public Vector3 attackingUnits;

    void Start() {
        renderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

        size = Size.random();
        flipX = RANDOM.Next(2) != 0;
        flipY = RANDOM.Next(2) != 0;
        angle = RANDOM.Next(4) * 90;

        gameObject.transform.localEulerAngles = new Vector3(0, 0, angle);
        gameObject.transform.localScale = new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1);

        switch (RANDOM.Next(3))
        {
            case 0:
                faction = Faction.FRIENDLY;
                break;
            case 1:
                faction = Faction.NEUTRAL;
                break;
            case 2:
                faction = Faction.ENEMY;
                break;
        }

        renderer.sprite = size.sprite;

        var villageTaken = Instantiate(villageTakenPrefab);
        villageTaken.transform.parent = gameObject.transform;
        villageTaken.transform.localPosition = new Vector3(0, 0, -1);
        var villageTakenScript = villageTaken.GetComponent(typeof (Village_Taken)) as Village_Taken;
        if (villageTakenScript)
        {
            villageTakenScript.Adapt();
        }
        
        var hexRenderer = transform.parent.gameObject.GetComponent<Renderer>();
        hexRenderer.material.color = faction.color;
    }
}
