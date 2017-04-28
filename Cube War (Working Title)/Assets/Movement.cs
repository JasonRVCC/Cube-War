using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Simple movement script will move the object based on key input, in the direction the object is facing.
/// Requires XMove, YMove, and ZMove to be set up in the input manager.
/// 
/// Also contains functions for the camera interface class to use to manipulate the camera.
/// </summary>

public class Movement : MonoBehaviour {
    public Vector3 mov;
    public float speed = 2;
    public bool focusSet = false;
    public Vector3 endPos;
    public Vector3 startPos;
    public float startTime;
    public float t;
    public float smoothTime = 1;
    public CameraZoom foc;


	void Update ()
    {
        mov = Vector3.zero;
        if (focusSet)
        {
            t = (Time.time - startTime) / smoothTime;
            if (t >= 1)
            {
                t = 1;
                focusSet = false;
            }
            mov = Vector3.Lerp(startPos, endPos, t);
        }
        endPos.x += Input.GetAxis("XMove");
        mov.x += Input.GetAxis("XMove");
        endPos.y += Input.GetAxis("YMove");
        mov.y += Input.GetAxis("YMove");
        endPos.z += Input.GetAxis("ZMove");
        mov.z += Input.GetAxis("ZMove");
        transform.localPosition += transform.rotation * mov * Time.deltaTime * speed * (1 + foc.getZoom() / 20);
	}



    public void setFocusDestination(Vector3 dest)
    {
        startPos = transform.position;
        endPos = dest;
        focusSet = true;
        startTime = Time.time;
    }
}
