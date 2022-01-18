using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandlerForSubLight : MonoBehaviour
{
    static public CameraHandlerForSubLight instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private bool isActive;
    [SerializeField] Camera cam;
    [SerializeField] Transform target;
    [SerializeField] private float distanceToTarget;
    [SerializeField] Vector3 directionVector;
    private Vector3 previousPosition;

    private void Update()
    {
        if (isActive == false) return;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically
            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;

            directionVector = (cam.transform.position - target.position);
            directionVector.Normalize();

        }
#elif (UNITY_IOS || UNITY_ANDROID)
        if (Input.touchCount != 1) return;
        if ((Input.GetMouseButtonDown(0)))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically
            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;

            directionVector = (cam.transform.position - target.position);
            directionVector.Normalize();
        }
#endif

    }


    public void Activate()
    {
        CameraHandler.instance.enabled = false;
        isActive = true;
    }
    public void Deactivate()
    {
        CameraHandler.instance.enabled = true;
        isActive = false;
    }
}
