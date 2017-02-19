using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	// Scaling factors for the whole game
	public float hScale = 3.0f;
	public float vScale = 2.0f;

	// Set terrain dimensions
	public int numberOfLanes = 50;
	public int numberOfRows = 50;

	public float treeDensity = 0.20f;

	// Main game components
	MeshTerrain meshTerrain;
	Sentinel oSentinel;
	Tree oTree;

	// Use this for initialization
	void Start ()
	{
		meshTerrain = GetComponentInChildren<MeshTerrain> ();
		meshTerrain.GenerateTerrain ();

		// Generate an army of sentinels
		oSentinel = GetComponentInChildren<Sentinel> ();
		List<Vector3> hc = meshTerrain.GetHighestPlatformCellPositions ();
		foreach (Vector3 v in hc)
			oSentinel.Create (v + new Vector3 (0, 0.075f * vScale, 0));

		// Spread some trees onto the terrain
		oTree = GetComponentInChildren<Tree> ();
		List<GameObject> pcs = meshTerrain.platforms;
		foreach (GameObject p in pcs)
			if (Random.value < treeDensity) oTree.Create (p.transform.position + new Vector3(0.5f * hScale, 0.05f * vScale, 0.5f * hScale));
		}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
