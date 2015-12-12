using UnityEngine;
using System.Collections;
using Random = System.Random;

public class SetupGame : MonoBehaviour {

    // Use this for initialization
    public Transform transform;
    public Camera camera;

    private static readonly Random random = new Random();

    public int oneIn = 3;

    void Start () {

        float maxHeight = 5;
        float maxWidth = 8;

        float unit = maxWidth / 5;

        for (float y = -maxHeight; y < maxHeight; y += unit)
        {
            for (float x = -maxWidth; x < maxWidth; x += unit)
            {
                if (random.Next(oneIn) == 0)
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
