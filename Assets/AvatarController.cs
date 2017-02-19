using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    Transform centerEye, rightTouch, laser;
    const float TravelSpeed = 10;

    // Use this for initialization
    void Start()
    {
        centerEye = transform.Find("TrackingSpace/CenterEyeAnchor");
        if (centerEye == null) Debug.Log("Center eye not found!");

        rightTouch = transform.Find("LocalAvatar/controller_right");
        if (rightTouch == null) Debug.Log("Right touch controller not found!");

        laser = transform.Find("LocalAvatar/controller_right/Laser");
        if (laser == null) Debug.Log("Laser not found!");
    }

    // Update is called once per frame
    void Update()
    {
        // Quick and dirty way to move avatar around
        Vector2 touchAxis1 = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * Time.deltaTime * TravelSpeed;
        Vector2 touchAxis2 = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) * Time.deltaTime * TravelSpeed;

        transform.position += centerEye.forward * touchAxis1.y + centerEye.right * touchAxis1.x;
        transform.position += Vector3.up * touchAxis2.y;
        transform.Rotate(0.0f, touchAxis2.x * TravelSpeed, 0.0f);

        // Make sure laser stops at the first target
        RaycastHit hit;
        var touched = Physics.Raycast(rightTouch.position, rightTouch.forward, out hit);

        if (touched)
        {
            float dist = Vector3.Distance(rightTouch.position, hit.point);
            laser.localPosition = new Vector3(0.0f, 0.0f, dist / 2);
            laser.transform.localScale = new Vector3(0.001f, 0.001f, dist);
        }

        // Quick and dirty way to select a platform, change its colour and move onto it
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && touched && hit.collider.tag == "Platform" && hit.normal == Vector3.up)
        {
            var color = hit.transform.gameObject.GetComponent<Renderer>().material.color;
            color = color == Color.green ? Color.white : Color.green;
            hit.transform.gameObject.GetComponent<Renderer>().material.color = color;
			transform.position = hit.transform.position + new Vector3 (0.0f, 1.5f, 0.0f);
        }

    }
}
