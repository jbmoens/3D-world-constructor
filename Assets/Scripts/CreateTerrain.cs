using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTerrain : MonoBehaviour {

	public GameObject block;

	[Range(1,100)]
	public int numberOfBlocks = 10;

	public float terrainHalfSize = 5f;

	public float blockMaximumWidth = 2f;
	public float blockMaximumDepth = 2f;
	public float blockMaximumHeight = 2f;

	// Use this for initialization
	void Start () {
		for (int i=0; i<numberOfBlocks; i++) {
			Vector3 pos = new Vector3 (Random.Range(-terrainHalfSize, terrainHalfSize), Random.Range(-terrainHalfSize, terrainHalfSize), 0f);
			GameObject b = (GameObject) Instantiate (block, pos, Quaternion.Euler(i,i,i));
			Vector3 size = new Vector3 (Random.Range (1f, blockMaximumWidth), Random.Range (1f, blockMaximumDepth), Random.Range (1f, blockMaximumHeight));
			b.transform.localScale = size;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
