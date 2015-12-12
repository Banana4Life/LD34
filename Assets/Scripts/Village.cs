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

    public GameObject gameObject;
    public SpriteRenderer renderer;
    public CircleCollider2D collider;
    public Transform transform;
    public Faction faction = Faction.NEUTRAL;
    public float radius;
    private static readonly Random random = new Random();

    public float minRadius = 0.3f;
    public int radiusSteps = 3;
    public float radiusStepScale = 0.12f;

    void Start()
    {
        radius = (random.Next(radiusSteps) * radiusStepScale + minRadius);
        collider.radius = radius;
        transform.localScale = new Vector3(radius * 2, radius * 2);

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
    }
                

	void Update ()
    {
        renderer.color = faction.color;
    }
}
