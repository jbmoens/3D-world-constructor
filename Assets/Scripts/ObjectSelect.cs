using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit))
			{
				if (hit.collider.tag == "Platform")
				{
					Debug.Log ("You hit a platform!");
					hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
				}
			}
		}
	}
}
