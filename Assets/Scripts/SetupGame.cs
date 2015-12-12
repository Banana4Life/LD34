using UnityEngine;
using System.Collections;
using Random = System.Random;

public class SetupGame : MonoBehaviour {

    // Use this for initialization
    public Transform transform;
    public Camera camera;

    private static readonly Random random = new Random();

    // 100 in X
    public int villageChance100InX = 300;

    public float maxHeight = 10;
    public float maxWidth = 16;

    public float steps = 1.6f;

    void Start () {
        for (float y = -maxHeight / 2; y < maxHeight / 2; y += steps)
        {
            for (float x = -maxWidth / 2; x < maxWidth / 2; x += steps)
            {
                if (random.Next(villageChance100InX) <= 100)
                {
                    Instantiate(transform, new Vector3(x, y, 0f), Quaternion.identity);
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
