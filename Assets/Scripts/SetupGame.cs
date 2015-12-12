using UnityEngine;
using System.Collections;
using Random = System.Random;

public class SetupGame : MonoBehaviour {

    // Use this for initialization
    public GameObject villagePrefab;



    private static readonly Random random = new Random();

    // 100 in X
    public int villageChance100InX = 300;

    public float maxHeight = 10;
    public float maxWidth = 16;

    public float steps = 1.6f;

    void Start () {
        for (float y = -maxHeight; y < maxHeight; y++)
        {
            for (float x = -maxWidth; x < maxWidth; x++)
            {
                if (random.Next(villageChance100InX) <= 100)
                {
                    var village = Instantiate(villagePrefab);
                    village.transform.position = new Vector3(x, y, 0); // TODO parent instead
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
