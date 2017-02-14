using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	// Main game components
	MeshTerrain meshTerrain;
	Sentinel sentinel;

	// Use this for initialization
	void Start ()
	{
		meshTerrain = GetComponentInChildren<MeshTerrain> ();
		meshTerrain.GenerateTerrain ();

        // Generate an army of sentinels
		sentinel = GetComponentInChildren<Sentinel> ();
		List<Vector3> hc = meshTerrain.GetHighestPlatformCellPositions ();
		foreach (Vector3 v in hc)
			sentinel.Create (v + new Vector3 (0, 0.05f, 0));
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
