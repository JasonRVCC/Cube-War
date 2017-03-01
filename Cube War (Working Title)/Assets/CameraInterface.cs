using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 
/// Camera interface for controlling the camera.
/// 
/// Contains functions for moving the camera's focus to specific points or objects.
/// 
/// </summary>

public static class CameraInterface {

    public static GameObject cameraAnchor;

    public static void Focus(Vector3 dest)
    {
        cameraAnchor.GetComponent<Movement>().setFocusDestination(dest);
    }

    public static void Focus(GameObject obj)
    {
        cameraAnchor.GetComponent<Movement>().setFocusDestination(obj.transform.position);
    }

    public static Vector3 getCameraFocus()
    {
        return cameraAnchor.transform.position;
    }
}
