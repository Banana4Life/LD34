using UnityEngine;
using System.Collections;

public class SetupGame : MonoBehaviour {

    // Use this for initialization
    public Transform transform;
	void Start () {

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Instantiate(transform, new Vector3(x * 2, y * 2, 0f), Quaternion.identity);
            }
        }
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
