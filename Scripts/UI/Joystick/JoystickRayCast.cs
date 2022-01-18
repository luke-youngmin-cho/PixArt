using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
enum e_PressFSM
{
    Idle,
    Clicked,
    Presssed,
    Dragging,
    Detached
}
public class JoystickRayCast : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler,IEndDragHandler
{
    static public JoystickRayCast instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Debug.LogError("joystick render graphic ray casting conflict");

        localCursor = new Vector2(0, 0);       
    }
    //Drag Orthographic top down camera here
    public Camera naviRenderCam;
    [HideInInspector]
    public bool isJoystickUsing;

    e_PressFSM pressFSM;
    Vector2 localCursor;
    
    Coroutine pointerCoroutine;
    Coroutine draggingCoroutine;
    delegate void PointerPressed();
    PointerPressed pointerPressed;
    delegate void PointerDragging();
    PointerDragging pointerDragging;

    // settings value ( set by SettingsManager in awake)
    [HideInInspector]public float pressedDetectionDelay;
    [HideInInspector]public float movingTerm;

    private Texture tex;
    private Rect r;
    // joystick panel
    [SerializeField] RectTransform joystickPanelRect;
    private void Start()
    {
        tex = GetComponent<RawImage>().texture;
        r = GetComponent<RawImage>().rectTransform.rect;
        

        pointerPressed = ClickEvent;
        pointerDragging = DragEvent;
        movingTerm = 1 / SettingsManager.instance.Touch_JoystickSpeed;
        pressedDetectionDelay = SettingsManager.instance.Touch_JoystickSensitivity;
    }
    void ClickEvent()
    {
        Vector2 tmpCursor;

        //Using the size of the texture and the local cursor, clamp the X,Y coords between 0 and width - height of texture
        float coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
        float coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);

        //Convert coordX and coordY to % (0.0, 0.0) ~ (1.0, 1.0) with respect to texture width and height
        float recalcX = coordX / tex.width;
        float recalcY = coordY / tex.height;

        Debug.Log($"recalced coord  = {recalcX}, {recalcY}");
        tmpCursor = new Vector2(recalcX, recalcY);
        CastRenderTextureRayToRender(tmpCursor);
    }
    void DragEvent()
    {
        Vector2 tmpCursor;

        //Using the size of the texture and the local cursor, clamp the X,Y coords between 0 and width - height of texture
        float coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
        float coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);

        //Convert coordX and coordY to % (0.0, 0.0) ~ (1.0, 1.0) with respect to texture width and height
        float recalcX = coordX / tex.width;
        float recalcY = coordY / tex.height;

        Debug.Log($"recalced coord  = {recalcX}, {recalcY}");
        tmpCursor = new Vector2(recalcX, recalcY);
        CalcProjectedCoord(localCursor);
        // todo -> call move cursor method
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (pressFSM == e_PressFSM.Presssed) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localCursor))
        {
            ClickEvent();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localCursor))
        {
            isJoystickUsing = true;
            pressFSM = e_PressFSM.Clicked;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {   
        pressFSM = e_PressFSM.Detached;
        isJoystickUsing = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localCursor))
        {
            pressFSM = e_PressFSM.Dragging;
            isJoystickUsing = true;
            Debug.Log("Start Drag!");
        }
            
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.position, eventData.pressEventCamera, out localCursor))
        {
            if (pressFSM != e_PressFSM.Dragging)
            {
                pressFSM = e_PressFSM.Dragging;
                isJoystickUsing = true;
            }   
        }   
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        pressFSM = e_PressFSM.Detached;
        isJoystickUsing = false;
    }
    IEnumerator DelayToChangePressedState()
    {
        yield return new WaitForSeconds(pressedDetectionDelay);
        if (isJoystickUsing == true)
        {
            pressFSM = e_PressFSM.Presssed;
        }
        pointerCoroutine = null;
    }
    IEnumerator PressingEventWithDelay(PointerPressed d_pointerPressed)
    {
        while ((pressFSM == e_PressFSM.Presssed) & (isJoystickUsing == true))
        {
            d_pointerPressed();
            yield return new WaitForSeconds(movingTerm);
        }
        pressFSM = e_PressFSM.Detached;
        pointerCoroutine = null;
    }
    IEnumerator DraggingEventWithDelay(PointerDragging d_pointerDragging)
    {
        while ((pressFSM == e_PressFSM.Dragging) & (isJoystickUsing == true))
        {
            d_pointerDragging();

            if (draggedToXplus) EditorUIManager.Instance.MoveNodeCurser_Xplus();
            if (draggedToXminus) EditorUIManager.Instance.MoveNodeCurser_Xminus();
            if (draggedToYplus) EditorUIManager.Instance.MoveNodeCurser_Yplus();
            if (draggedToYminus) EditorUIManager.Instance.MoveNodeCurser_Yminus();
            if (draggedToZplus) EditorUIManager.Instance.MoveNodeCurser_Zplus();
            if (draggedToZminus) EditorUIManager.Instance.MoveNodeCurser_Zminus();

            yield return new WaitForSeconds(movingTerm);
        }
        pressFSM = e_PressFSM.Detached;
        draggingCoroutine = null;
    }
    private void Update()
    {
        switch (pressFSM)
        {
            case e_PressFSM.Idle:
                break;
            case e_PressFSM.Clicked:
                CameraHandler.instance.DisableHandling();
                if(pointerCoroutine == null) pointerCoroutine = StartCoroutine(DelayToChangePressedState());
                break;
            case e_PressFSM.Presssed:
                if (pointerCoroutine == null) pointerCoroutine = StartCoroutine(PressingEventWithDelay(pointerPressed));
                break;
            case e_PressFSM.Dragging:
                if (draggingCoroutine == null) draggingCoroutine = StartCoroutine(DraggingEventWithDelay(pointerDragging));
                break;
            case e_PressFSM.Detached:
                if (pointerCoroutine != null) 
                {
                    StopCoroutine(pointerCoroutine);
                    pointerCoroutine = null;
                }
                CameraHandler.instance.EnableHandling();
                pressFSM = e_PressFSM.Idle;
                break;
            default:
                break;
        }
    }

    private void CastRenderTextureRayToRender(Vector2 localCursor)
    {
        Ray renderTextureRay = naviRenderCam.ScreenPointToRay(new Vector2(localCursor.x * naviRenderCam.pixelWidth, localCursor.y * naviRenderCam.pixelHeight));

        RaycastHit renderTextureHit;

        if (Physics.Raycast(renderTextureRay, out renderTextureHit, Mathf.Infinity))
        {
            if (renderTextureHit.collider != null)
            {
                //Debug.Log("renderTextureHit: " + renderTextureHit.collider.gameObject);
                renderTextureHit.collider.gameObject.GetComponent<JoystickButton>().RayCastedByClicking(); // Q? better way?
            }            
        }
    }

    public void SetPressedDetectionDelayTime(Slider slider)
    {
        pressedDetectionDelay = slider.value;
        PlayerPrefs.SetFloat("JoystickPressedDetectionDelayTime", pressedDetectionDelay);
    }

    public void SetMovingTermWithSpeed(Slider slider)
    {
        movingTerm = 0.1f / slider.value;
        PlayerPrefs.SetFloat("JoystickMovingTerm", movingTerm);
    }

    //===================================================================================================================
    // 3D Motion Touch Calculation
    //===================================================================================================================
    [SerializeField] Transform coordZero;
    [SerializeField] Transform coordXplus;
    [SerializeField] Transform coordXminus;
    [SerializeField] Transform coordYplus;
    [SerializeField] Transform coordYminus;
    [SerializeField] Transform coordZplus;
    [SerializeField] Transform coordZminus;
    bool draggedToXplus;
    bool draggedToXminus;
    bool draggedToYplus;
    bool draggedToYminus;
    bool draggedToZplus;
    bool draggedToZminus;
    [SerializeField] RectTransform debuggingImageTransform_Xplus;
    [SerializeField] RectTransform debuggingImageTransform_Xminus;
    [SerializeField] RectTransform debuggingImageTransform_Yplus;
    [SerializeField] RectTransform debuggingImageTransform_Yminus;
    [SerializeField] RectTransform debuggingImageTransform_Zplus;
    [SerializeField] RectTransform debuggingImageTransform_Zminus;

    Vector2 touchWindow = new Vector2(30f, 30f);
    float dragOffset = 2.0f;
    float dragOffset2 = 2.6f;
    /// <summary>
    /// this method will call when user drag joystick.
    /// Calculate dragged position delta and scale to coordinate (0.0, 0.0) ~ (1.0, 1.0)
    /// </summary>
    private void CalcProjectedCoord(Vector2 cursor)
    {
        // convert coord based on camera.
        Vector3 zero_cam = naviRenderCam.WorldToScreenPoint(coordZero.position);
        Vector3 xPlus_cam = naviRenderCam.WorldToScreenPoint(coordXplus.position);
        Vector3 xMinus_cam = naviRenderCam.WorldToScreenPoint(coordXminus.position);
        Vector3 yPlus_cam = naviRenderCam.WorldToScreenPoint(coordYplus.position);
        Vector3 yMinus_cam = naviRenderCam.WorldToScreenPoint(coordYminus.position);
        Vector3 zPlus_cam = naviRenderCam.WorldToScreenPoint(coordZplus.position);
        Vector3 zMinus_cam = naviRenderCam.WorldToScreenPoint(coordZminus.position);

        xPlus_cam  -= new Vector3(0.5f, 0.5f, 0f) * naviRenderCam.pixelWidth;
        xMinus_cam -= new Vector3(0.5f, 0.5f, 0f) * naviRenderCam.pixelWidth;
        yPlus_cam  -= new Vector3(0.5f, 0.5f, 0f) * naviRenderCam.pixelWidth;
        yMinus_cam -= new Vector3(0.5f, 0.5f, 0f) * naviRenderCam.pixelWidth;
        zPlus_cam  -= new Vector3(0.5f, 0.5f, 0f) * naviRenderCam.pixelWidth;
        zMinus_cam -= new Vector3(0.5f, 0.5f, 0f) * naviRenderCam.pixelWidth;

        Vector3 xPlus_cam2 = xPlus_cam * dragOffset2;
        Vector3 xMinus_cam2 = xMinus_cam * dragOffset2;
        Vector3 yPlus_cam2 = yPlus_cam * dragOffset2;
        Vector3 yMinus_cam2 = yMinus_cam * dragOffset2;
        Vector3 zPlus_cam2 = zPlus_cam * dragOffset2;
        Vector3 zMinus_cam2 = zMinus_cam * dragOffset2;

        xPlus_cam *= dragOffset;
        xMinus_cam *= dragOffset;
        yPlus_cam *= dragOffset;
        yMinus_cam *= dragOffset;
        zPlus_cam *= dragOffset;
        zMinus_cam *= dragOffset;

        // for debugging projected joystick
        //debuggingImageTransform_Xplus.localPosition = xPlus_cam2;
        //debuggingImageTransform_Xminus.localPosition = xMinus_cam2;
        //debuggingImageTransform_Yplus.localPosition = yPlus_cam2;
        //debuggingImageTransform_Yminus.localPosition = yMinus_cam2;
        //debuggingImageTransform_Zplus.localPosition = zPlus_cam2;
        //debuggingImageTransform_Zminus.localPosition = zMinus_cam2;

        // Deciding direction to move.
        draggedToXplus  = false;
        draggedToXminus = false;
        draggedToYplus  = false;
        draggedToYminus = false;
        draggedToZplus  = false;
        draggedToZminus = false;

        if(((cursor.x <= xPlus_cam.x + touchWindow.x) &
            (cursor.x >= xPlus_cam.x - touchWindow.x) &
            (cursor.y <= xPlus_cam.y + touchWindow.y) &
            (cursor.y >= xPlus_cam.y - touchWindow.y)) |
           ((cursor.x <= xPlus_cam2.x + touchWindow.x) &
            (cursor.x >= xPlus_cam2.x - touchWindow.x) &
            (cursor.y <= xPlus_cam2.y + touchWindow.y) &
            (cursor.y >= xPlus_cam2.y - touchWindow.y)))

        {
            draggedToXplus = true;
        }
        if (((cursor.x <= xMinus_cam.x + touchWindow.x) &
             (cursor.x >= xMinus_cam.x - touchWindow.x) &
             (cursor.y <= xMinus_cam.y + touchWindow.y) &
             (cursor.y >= xMinus_cam.y - touchWindow.y)) |
            ((cursor.x <= xMinus_cam2.x + touchWindow.x) &
             (cursor.x >= xMinus_cam2.x - touchWindow.x) &
             (cursor.y <= xMinus_cam2.y + touchWindow.y) &
             (cursor.y >= xMinus_cam2.y - touchWindow.y)))
        {
            draggedToXminus = true;
        }
        if (((cursor.x <= yPlus_cam.x + touchWindow.x) &
             (cursor.x >= yPlus_cam.x - touchWindow.x) &
             (cursor.y <= yPlus_cam.y + touchWindow.y) &
             (cursor.y >= yPlus_cam.y - touchWindow.y)) |
            ((cursor.x <= yPlus_cam2.x + touchWindow.x) &
             (cursor.x >= yPlus_cam2.x - touchWindow.x) &
             (cursor.y <= yPlus_cam2.y + touchWindow.y) &
             (cursor.y >= yPlus_cam2.y - touchWindow.y)))
        {
            draggedToYplus = true;
        }
        if (((cursor.x <= yMinus_cam.x + touchWindow.x) &
             (cursor.x >= yMinus_cam.x - touchWindow.x) &
             (cursor.y <= yMinus_cam.y + touchWindow.y) &
             (cursor.y >= yMinus_cam.y - touchWindow.y)) |
            ((cursor.x <= yMinus_cam2.x + touchWindow.x) &
             (cursor.x >= yMinus_cam2.x - touchWindow.x) &
             (cursor.y <= yMinus_cam2.y + touchWindow.y) &
             (cursor.y >= yMinus_cam2.y - touchWindow.y)))
        {
            draggedToYminus = true;
        }
        if (((cursor.x <= zPlus_cam.x + touchWindow.x) &
             (cursor.x >= zPlus_cam.x - touchWindow.x) &
             (cursor.y <= zPlus_cam.y + touchWindow.y) &
             (cursor.y >= zPlus_cam.y - touchWindow.y)) |
            ((cursor.x <= zPlus_cam2.x + touchWindow.x) &
             (cursor.x >= zPlus_cam2.x - touchWindow.x) &
             (cursor.y <= zPlus_cam2.y + touchWindow.y) &
             (cursor.y >= zPlus_cam2.y - touchWindow.y)))
        {
            draggedToZplus = true;
        }
        if (((cursor.x <= zMinus_cam.x + touchWindow.x) &
             (cursor.x >= zMinus_cam.x - touchWindow.x) &
             (cursor.y <= zMinus_cam.y + touchWindow.y) &
             (cursor.y >= zMinus_cam.y - touchWindow.y)) |
            ((cursor.x <= zMinus_cam2.x + touchWindow.x) &
             (cursor.x >= zMinus_cam2.x - touchWindow.x) &
             (cursor.y <= zMinus_cam2.y + touchWindow.y) &
             (cursor.y >= zMinus_cam2.y - touchWindow.y)))
        {
            draggedToZminus = true;
        }
        
        /*Debug.Log($"x+ {draggedToXplus }");
        Debug.Log($"x- {draggedToXminus}");
        Debug.Log($"y+ {draggedToYplus }");
        Debug.Log($"y- {draggedToYminus}");
        Debug.Log($"z+ {draggedToZplus }");
        Debug.Log($"z- {draggedToZminus}");*/
    }
    public void SetJoystisckPrecision(float precision)
    {
        touchWindow = new Vector2(precision, precision);
    }
}

