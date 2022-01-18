using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine;

public enum e_CameraState
{
    Idle,
    Moving,
    Zooming
}

public enum e_CameraTarget
{
    target1,
    target2,
    target3
}

public class CameraHandler : MonoBehaviour
{
    static public CameraHandler instance;
    private e_CameraState cameraState;
    [SerializeField] Vector3 defaultAngle;
    [SerializeField] float defaultDistanceToTarget;
    private void Awake()
    {
        if (instance == null) instance = this;
        isCameraHandlingEnabled = true;
        cameraState = e_CameraState.Idle;
        distanceToTarget = defaultDistanceToTarget;
    }
    [SerializeField] private Camera cam;
    public JoystickCamera naviCam;
    [SerializeField] Skybox skybox;
    Transform initTransform;
    Quaternion initQuaternion;

    // moving elements
    [SerializeField] bool focusOnCursor;
    private Vector3 targetPosition;
    [SerializeField] private Transform target1; // cubeDesign Transform
    private Transform target2; // cubeDesign Cursor Trnasform
    [SerializeField] private Transform targetResetPoint;
    private float distanceToTarget;
    private Vector3 previousPosition;
    public Vector3 directionVector;
    e_CameraTarget cameraTarget;

    // zoom elements
    [SerializeField] float initFieldOfView;
    public float zoomFactor;

    public bool isCameraHandlingEnabled;
    private bool isInBound;
    private void Start()
    {
        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));
        cameraTarget = e_CameraTarget.target1;
        cam.fieldOfView = initFieldOfView;
        zoomFactor = SettingsManager.instance.Touch_ZoomSpeed;
        if (focusOnCursor == true) 
            ToggleCameraTarget_1and2();
        initTransform = cam.transform;
        initQuaternion = new Quaternion(cam.transform.rotation.x,
                                        cam.transform.rotation.y,
                                        cam.transform.rotation.z,
                                        cam.transform.rotation.w);
    }
    void Update()
    {
        UpdateCameraState();
        UpdateCameraTarget();
        MoveCamera();
        ZoomCamera();
    }
    
    private void UpdateCameraState()
    {
        if (Input.touchCount == 0) cameraState = e_CameraState.Idle;
    }
    private void UpdateCameraTarget()
    {
        switch (cameraTarget)
        {
            case e_CameraTarget.target1:
                targetPosition = target1.position;
                break;
            case e_CameraTarget.target2:

                if(EditorUIManager.Instance.cubeDesigneInstance != null)
                {                     
                    if (EditorUIManager.Instance.cubeDesigneInstance.isCursorExists)
                        targetPosition = EditorUIManager.Instance.cubeDesigneInstance.GetCursorTranslation().Value;
                    return;
                }
                
                targetPosition = target1.position;
                break;
            case e_CameraTarget.target3:
                break;
            default:
                break;
        }
    }
    private void MoveCamera()
    {
        //Debug.Log($"{isInBound} , {isCameraHandlingEnabled}, {NaviRenderGraphicRayCasting.instance.isJoystickUsing}");
        if (isInBound == false) return;
        if (isCameraHandlingEnabled == false) return;
        if (JoystickRayCast.instance.isJoystickUsing == true) return;

#if (UNITY_EDITOR)
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            cameraState = e_CameraState.Moving;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically
            cam.transform.position = targetPosition;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;

            directionVector = (cam.transform.position - targetPosition);
            directionVector.Normalize();

            naviCam.OnMouseButton();
        }

#elif (UNITY_IOS || UNITY_ANDROID)

        if (Input.touchCount != 1) return;
       
        if ((Input.GetMouseButtonDown(0)) &(cameraState == e_CameraState.Idle)) 
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            cameraState = e_CameraState.Moving;
        }
        else if (Input.GetMouseButton(0) & (cameraState == e_CameraState.Moving))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically
            cam.transform.position = targetPosition;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;

            directionVector = (cam.transform.position - targetPosition);
            directionVector.Normalize();

            naviCam.OnMouseButton();
        }
#endif
    }
    private void ZoomCamera()
    {
        if (isInBound == false) return;
        if (isCameraHandlingEnabled == false) return;
#if (UNITY_EDITOR)
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            distanceToTarget -= zoomFactor;
            cam.transform.Translate(new Vector3(0, 0, zoomFactor));
            //previousPosition = newPosition;

            cam.fieldOfView -= zoomFactor;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            distanceToTarget += zoomFactor;
            cam.transform.Translate(new Vector3(0, 0, -zoomFactor));
            //previousPosition = newPosition;

            cam.fieldOfView += zoomFactor;
        }
#elif (UNITY_IOS || UNITY_ANDROID)
        if (Input.touchCount != 2) return;
        
        float disX = 0;
        float disY = 0;
        cameraState = e_CameraState.Zooming;

        if (Input.GetTouch(0).position.x < Input.GetTouch(1).position.x)
        {
            disX = Input.GetTouch(0).deltaPosition.x - Input.GetTouch(1).deltaPosition.x;
        }
        else
        {
            disX = - Input.GetTouch(0).deltaPosition.x + Input.GetTouch(1).deltaPosition.x;
        }

        if (Input.GetTouch(0).position.y < Input.GetTouch(1).position.y)
        {
            disY = Input.GetTouch(0).deltaPosition.y - Input.GetTouch(1).deltaPosition.y;
        }
        else
        {
            disY = -Input.GetTouch(0).deltaPosition.y + Input.GetTouch(1).deltaPosition.y;
        }
        var zoom = disX + disY;

        cam.fieldOfView += zoom*zoomFactor;
        
#endif
    }
    public void SetCameraDefaultSettings()
    {
        SetCameraAngleDefault();
        SetDistanceDefault();
    }
    public void SetCameraAngle(Vector3 angle)
    {        
        gameObject.transform.rotation.SetEulerAngles(Mathf.Deg2Rad*(angle));
        gameObject.transform.position = target1.position;

        gameObject.transform.position = gameObject.transform.forward * distanceToTarget;
    }
    public void SetCameraAngleDefault()
    {
        // initialize
        cam.transform.position = initTransform.position;
        cam.transform.rotation = initQuaternion;

        // rotation
        float rotationAroundYAxis = defaultAngle.y;
        float rotationAroundXAxis = defaultAngle.x;
        cam.transform.position = targetPosition;

        cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
        cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

        directionVector = (cam.transform.position - targetPosition);
        directionVector.Normalize();

        naviCam.OnMouseButton();
    }
    public void SetDistanceDefault()
    {
        distanceToTarget = defaultDistanceToTarget;
        cam.fieldOfView = initFieldOfView;
    }

    public void Refresh()
    {
        UpdateCameraTarget();
        cam.transform.position = targetPosition;
        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));
    }
    public void ResetFieldOfView()
    {
        cam.fieldOfView = initFieldOfView;
    }
    private void DragCamera()
    {
        
    }
    public void ResetCamera()
    {
        targetPosition = targetResetPoint.position;
        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 direction = previousPosition - newPosition;

        float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
        float rotationAroundXAxis = direction.y * 180; // camera moves vertically
        cam.transform.position = targetPosition;

        cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
        cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

        previousPosition = newPosition;

        directionVector = (cam.transform.position - targetPosition);
        directionVector.Normalize();

        naviCam.OnMouseButton();
    }
    public void TranslateCamera(string plane)
    {
        switch (plane)
        {
            case "XY":
                break;
            case "XZ":
                break;
            case "YZ":
                break;
            case "XYZ":
                break;
            default:
                break;
        }
    }

    public void ChangeCameraTarget(e_CameraTarget targetType)
    {
        cameraTarget = targetType;
    }

    public void ToggleCameraTarget_1and2()
    {
        if(cameraTarget == e_CameraTarget.target1)
        {
            cameraTarget = e_CameraTarget.target2;
        }
        else
        {
            cameraTarget = e_CameraTarget.target1;
        }
    }

    public void TouchedInBound(bool isIt)
    {
        isInBound = isIt;
    }
    
    public void ChangeClearFlag(string flagTypeName)
    {
        string tmpFlagTypeName = flagTypeName.ToLower();
        switch (tmpFlagTypeName)
        {
            case "skybox":
                cam.clearFlags = CameraClearFlags.Skybox;
                break;
            case "solidcolor":
                cam.clearFlags = CameraClearFlags.SolidColor;
                break;
            default:
                Debug.LogError("CameraHandler::ChangeClearFlag() input is wrong. maybe wrong spelling");
                break;
        }
    }
    public void ChangeCameraColor(Color color)
    {
        cam.backgroundColor = color;
    }

    public void ChangeSkyBox(Material skyboxMat)
    {
        skybox.material = skyboxMat;
    }

    public void EnableHandling()
    {
        isCameraHandlingEnabled = true;
    }

    public void DisableHandling()
    {
        isCameraHandlingEnabled = false;
    }
    public void TurnTheCameraAtTheAngle()
    {

    }

    public void SetZoomFactor(Slider slider)
    {   
        zoomFactor = slider.value;
    }
}