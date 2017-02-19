using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {
	
	public GameObject treePrefab;
	List<GameObject> trees;

	// Use this for initialization
	void Start ()
	{
		trees = new List<GameObject>();
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void Create (Vector3 position)
	{
		// Instantiate Blender object and rotate as required
		var t = Instantiate (treePrefab, position, Quaternion.Euler (0, 0, 0));
		trees.Add(t);
	}
}
