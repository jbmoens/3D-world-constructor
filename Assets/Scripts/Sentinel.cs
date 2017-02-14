using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : MonoBehaviour
{
	public GameObject sentinelPrefab;
    List<GameObject> sentinels;

	// Use this for initialization
	void Start ()
	{
        sentinels = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        foreach (GameObject s in sentinels)
            s.transform.Rotate(new Vector3(0, 0, 0.1f));
    }

    public void Create (Vector3 position)
	{
		// Instantiate Blender object and rotate as required
		var s = Instantiate (sentinelPrefab, position, Quaternion.Euler (-90, 0, 0));
        sentinels.Add(s);
	}
}
