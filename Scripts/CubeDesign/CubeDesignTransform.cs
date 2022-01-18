using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CubeDesignTransform : MonoBehaviour
{
    public static CubeDesignTransform instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // rotation
    public float rotationSpeed = 1f;
    // mouse drag event to rotate cube
    /*private void OnMouseDrag()
    {
        float XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.Rotate(Vector3.down, XaxisRotation);
        transform.Rotate(Vector3.left, YaxisRotation);
        RotateNavigator();
    }

    public void RotateNavigator()
    {
        navigator.rotation = transform.rotation;
    }*/

}
