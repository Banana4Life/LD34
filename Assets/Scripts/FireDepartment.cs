using UnityEngine;
using System.Collections;

public class FireDepartment : MonoBehaviour
{

    public ParticleSystem smokingHouse;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (gameObject.transform.childCount == 0)
	    {
            /*if (smokingHouse.isPlaying) */smokingHouse.Stop();
            Destroy(gameObject);
	    }
	}
}
