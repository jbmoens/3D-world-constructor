using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaptopCamera : MonoBehaviour
{
	const float TravelSpeed = 10.0f;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Crude way of selecting a platform and changing its colour
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit) && hit.collider.tag == "Platform" && hit.normal == Vector3.up)
				hit.transform.gameObject.GetComponent<Renderer> ().material.color = Color.green;
		}

		// Simple key pressed to move the camera around the scene
		if (Input.GetKey (KeyCode.UpArrow))
			transform.position += transform.forward * Time.deltaTime * TravelSpeed;
		if (Input.GetKey (KeyCode.DownArrow))
			transform.position -= transform.forward * Time.deltaTime * TravelSpeed;
		if (Input.GetKey (KeyCode.RightArrow))
			transform.Rotate (0.0f, 10.0f * Time.deltaTime * TravelSpeed, 0.0f);
		if (Input.GetKey (KeyCode.LeftArrow))
			transform.Rotate (0.0f, -10.0f * Time.deltaTime * TravelSpeed, 0.0f);
		if (Input.GetKey ("a"))
			transform.position -= transform.right * Time.deltaTime * TravelSpeed;
		if (Input.GetKey ("d"))
			transform.position += transform.right * Time.deltaTime * TravelSpeed;
		if (Input.GetKey ("w"))
			transform.position += transform.up * Time.deltaTime * TravelSpeed;
		if (Input.GetKey ("s"))
			transform.position -= transform.up * Time.deltaTime * TravelSpeed;
		if (Input.GetKey ("e"))
			transform.Rotate (10.0f * Time.deltaTime * TravelSpeed, 0.0f, 0.0f);
		if (Input.GetKey ("q"))
			transform.Rotate (-10.0f * Time.deltaTime * TravelSpeed, 0.0f, 0.0f);
		if (Input.GetKey ("z"))
			transform.Rotate (0.0f, 0.0f, 10.0f * Time.deltaTime * TravelSpeed);
		if (Input.GetKey ("x"))
			transform.Rotate (0.0f, 0.0f, -10.0f * Time.deltaTime * TravelSpeed);
		if (Input.GetKey ("x") && Input.GetKey ("z")) {
			var angles = transform.localEulerAngles;
			angles.x = 0.0f;
			angles.z = 0.0f;
			transform.localEulerAngles = angles;
		}
	}
}
