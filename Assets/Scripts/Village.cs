using System;
using UnityEngine;
using System.Collections;
using Random = System.Random;

public class Village : MonoBehaviour
{
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
    
    public GameObject villageTakenPrefab;
    private SpriteRenderer renderer;
    public Faction faction = Faction.NEUTRAL;
    public float radius;
    private static readonly Random random = new Random();

    public static readonly float MIN_RADIUS = 0.3f;
    public static readonly int RADIUS_STEPS = 3;
    public static readonly float RADIUS_STEP_SCALE = 0.12f;

    void Start()
    {
        renderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

        radius = (random.Next(RADIUS_STEPS) * RADIUS_STEP_SCALE + MIN_RADIUS);
        //collider.radius = radius;
        //transform.localScale = new Vector3(radius * 2, radius * 2);

        switch (random.Next(3))
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

        var villageTaken = Instantiate(villageTakenPrefab);
        villageTaken.transform.parent = gameObject.transform;
        villageTaken.transform.localPosition = new Vector3(0, 0, -1);
        var villageTakenScript = villageTaken.GetComponent(typeof (Village_Taken)) as Village_Taken;
        if (villageTakenScript)
        {
            villageTakenScript.Adapt();
        }

        renderer.color = faction.color;
        var hexRenderer = transform.parent.gameObject.GetComponent<Renderer>();
        hexRenderer.material.color = faction.color;
    }
}
