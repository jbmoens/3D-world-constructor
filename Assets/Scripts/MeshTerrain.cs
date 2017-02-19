using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MeshTerrain : MonoBehaviour
{
	// The game controller object
	GameController gc;

	// Game object representing a platform cell
	public GameObject platformCell;

	// List of platform game objects
	public List<GameObject> platforms;

	// Some public parameters
	public bool ShowPlatforms = true;
    const float pOffset = 0.01f;

	// Perlin noise scaling factors
	public float PerlinNoiseHzFactor = 5.0f;
	public float PerlinNoiseVtFactor = 10.0f;

	// Set terrain dimensions
	const int numberOfLanes = 50;
	const int numberOfRows = 50;
	float hScale;
	float vScale;
	Vector3 corner1;
	Vector3 corner2;
	Vector3 corner3;

	// Derive some helpful constants
	const int numberOfVertices = numberOfLanes * numberOfRows * 6;

	// Define local mesh components
	Vector3[] vertices = new Vector3[numberOfVertices];
	int[] triangles = new int[numberOfVertices];
	Vector3[] normals = new Vector3[numberOfVertices];
	Vector2[] uvs = new Vector2[numberOfVertices];

	// Define local altitude map
	float[,] heights = new float[numberOfLanes + 1, numberOfRows + 1];

	// Define map of platform cells
	// Values:
	// 		0 and positive: height of the platform (in integer increments)
	// 		-1 or -2 means no platform in that spot (-2 is for a reverse triangle drawing pattern)
	int[,] pCells = new int[numberOfLanes, numberOfRows];

    // Use this for initialization
    void Start ()
	{
		platforms = new List<GameObject> ();
		gc = GetComponentInParent<GameController> ();
		hScale = gc.hScale;
		vScale = gc.vScale;
		corner1 = new Vector3 (0, 0, hScale);
		corner2 = new Vector3 (hScale, 0, hScale);
		corner3 = new Vector3 (hScale, 0, 0);
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void GenerateTerrain ()
	{
		// Get handle of the GameObject's mesh
		var mf = GetComponent<MeshFilter> ();
		var mesh = new Mesh ();
		mf.mesh = mesh;

		SetBackgroundHeights ();
		SetPlatformCells ();
		AdjustLocalHeights ();
		BuildTerrain ();

		// Assign the mesh
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uvs;

		var mc = GetComponent<MeshCollider> ();
		mc.sharedMesh = mesh;

		InstantiatePlatformCells ();
	}

	public List<Vector3> GetHighestPlatformCellPositions ()
	{
		var highestCoordinates = new List<Vector3>();
		int highest = -1;

		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++) {
				if (pCells [i, j] > highest) {
					highest = pCells [i, j];
					highestCoordinates.Clear ();
				}
				if (pCells [i, j] == highest)
					highestCoordinates.Add (new Vector3 ((i+0.5f)*hScale, highest*vScale, (j+0.5f)*hScale));
			}
		return highestCoordinates;
	}

	void SetPlatformCells ()
	{
		// Set all spots to empty cells
		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++)
				pCells [i, j] = -1;

		if (!ShowPlatforms)
			return;

/*		// Set border cells to lowest platform possible
		for (int j = 0; j < numberOfRows; j++)
			pCells [0, j] = pCells [numberOfLanes - 1, j] = 0;
		for (int i = 0; i < numberOfLanes; i++)
			pCells [i, 0] = pCells [i, numberOfRows - 1] = 0;

		// Test with a cross pattern
		for (int j = 0; j < numberOfRows; j++)
			pCells [numberOfLanes / 2, j] = 5;
		for (int i = 0; i < numberOfLanes; i++)
			pCells [i, numberOfRows / 2] = 10;
*/

		// Set platform cell heights
		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++)
				pCells [i, j] = (int)PerlinNoise (j, i);

		// Remove adjacent cells that do not share the same height
		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++)
				if (pCells [i, j] != -1) {
					if (i != numberOfLanes - 1 && pCells [i, j] != pCells [i + 1, j])
						pCells [i + 1, j] = -1;
					if (j != numberOfRows - 1 && pCells [i, j] != pCells [i, j + 1])
						pCells [i, j + 1] = -1;
					if (i != numberOfLanes - 1 && j != numberOfRows - 1 && pCells [i, j] != pCells [i + 1, j + 1])
						pCells [i + 1, j + 1] = -1;
					if (i != 0 && j != numberOfRows - 1 && pCells [i, j] != pCells [i - 1, j + 1])
						pCells [i - 1, j + 1] = -1;
				}

		// Change triangle drawing pattern for relevant corners
		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++) {
				bool changePattern = false;
				if (pCells [i, j] == -1) {
					if (i != numberOfLanes - 1 && j != numberOfRows - 1 && (
					        (pCells [i + 1, j + 1] > -1 && pCells [i, j + 1] < 0 && pCells [i + 1, j] < 0)
					        ||
					        (pCells [i, j + 1] > -1 && pCells [i + 1, j] > -1)
					    ))
						changePattern = true;
					if (i != 0 && j != 0 && (
					        (pCells [i - 1, j - 1] > -1 && pCells [i, j - 1] < 0 && pCells [i - 1, j] < 0)
					        ||
					        (pCells [i, j - 1] > -1 && pCells [i - 1, j] > -1)
					    ))
						changePattern = true;
				}
				if (changePattern)
					pCells [i, j] = -2;
			}
	}

	float PerlinNoise (int j, int i)
	{
		return vScale * PerlinNoiseVtFactor * Mathf.PerlinNoise (i * PerlinNoiseHzFactor / numberOfLanes, j * PerlinNoiseHzFactor / numberOfRows);
	}

	void SetBackgroundHeights ()
	{
		for (int j = 0; j < numberOfRows + 1; j++)
			for (int i = 0; i < numberOfLanes + 1; i++)
				heights [i, j] = PerlinNoise (j, i);
	}

	void AdjustLocalHeights ()
	{
		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++)
				if (pCells [i, j] > -1)
					heights [i, j] = heights [i + 1, j] = heights [i, j + 1] = heights [i + 1, j + 1] = vScale * pCells [i, j];
	}

	void BuildTerrain ()
	{
		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++)
				AddCellToMesh (i, j);
	}

	void AddCellToMesh (int i, int j)
	{
		var index = j * numberOfLanes + i;
		var v_index = index * 6;
		var basePosition = new Vector3 (i * hScale, 0, j * hScale);

		// Draw triangles based on corner pattern
		vertices [v_index] = basePosition + new Vector3 (0, heights [i, j], 0);
		vertices [v_index + 1] = basePosition + corner1 + new Vector3 (0, heights [i, j + 1], 0);
		if (pCells [i, j] == -1) {
			vertices [v_index + 2] = basePosition + corner3 + new Vector3 (0, heights [i + 1, j], 0);
			vertices [v_index + 3] = basePosition + corner3 + new Vector3 (0, heights [i + 1, j], 0);
			vertices [v_index + 4] = basePosition + corner1 + new Vector3 (0, heights [i, j + 1], 0);
			vertices [v_index + 5] = basePosition + corner2 + new Vector3 (0, heights [i + 1, j + 1], 0);
		} else {
			vertices [v_index + 2] = basePosition + corner2 + new Vector3 (0, heights [i + 1, j + 1], 0);
			vertices [v_index + 3] = basePosition + new Vector3 (0, heights [i, j], 0);
			vertices [v_index + 4] = basePosition + corner2 + new Vector3 (0, heights [i + 1, j + 1], 0);
			vertices [v_index + 5] = basePosition + corner3 + new Vector3 (0, heights [i + 1, j], 0);
		}

		triangles [v_index] = v_index;
		triangles [v_index + 1] = v_index + 1;
		triangles [v_index + 2] = v_index + 2;

		triangles [v_index + 3] = v_index + 3;
		triangles [v_index + 4] = v_index + 4;
		triangles [v_index + 5] = v_index + 5;

		var norm1 = CalculateNormal (vertices [v_index], vertices [v_index + 1], vertices [v_index + 2]);
		for (int k = 0; k < 3; k++)
			normals [v_index + k] = norm1;
		var norm2 = CalculateNormal (vertices [v_index + 3], vertices [v_index + 4], vertices [v_index + 5]);
		for (int k = 3; k < 6; k++)
			normals [v_index + k] = norm2;

		uvs [v_index] = Vector2.zero;
		uvs [v_index + 1] = Vector2.up;
		uvs [v_index + 2] = Vector2.right;
		uvs [v_index + 3] = Vector2.right;
		uvs [v_index + 4] = Vector2.up;
		uvs [v_index + 5] = Vector2.one;
	}

	Vector3 CalculateNormal (Vector3 v1, Vector3 v2, Vector3 v3)
	{
		var side1 = v2 - v1;
		var side2 = v3 - v1;
		var perp = Vector3.Cross (side1, side2).normalized;
		return perp;
	}

	void InstantiatePlatformCells ()
	{
		for (int j = 0; j < numberOfRows; j++)
			for (int i = 0; i < numberOfLanes; i++)
				if (pCells [i, j] > -1) {
					var h = pCells [i, j] * vScale;
					var pc = Instantiate (platformCell, new Vector3 (i * hScale, h + pOffset, j * hScale), Quaternion.identity);
					pc.transform.localScale = new Vector3 (hScale, vScale, hScale);
					platforms.Add (pc);
				}
	}
}
