using DOTS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum e_RectPositionType
{
    Middle,
    LeftTop,
    RightTop,
    LeftBottom,
    RightBottom,
    Top,
    Bottom,
    Left,
    Right
}

public class EditorUIManager : MonoBehaviour
{
    #region singleton
    static public EditorUIManager Instance;
    private void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
    }
    #endregion
    //---------init-------------
    public CMDState initState = CMDState.BUSY;
    Coroutine coroutine;
    //--------------------------
    public CubeDesign cubeDesign;
    public Transform whereYouWantToCreateCubeDesign;
    [HideInInspector]
    public CubeDesign cubeDesigneInstance;

    private ScreenOrientation screenOrientation;

    private int resolution;

    //Objects what you want to on off.
    public List<GameObject> objectsWhatYouWantToOnOffDependOnCubeDesign;
    public List<GameObject> objectsWhatYouWantToRefresh;

    /// <summary>
    /// UI managing
    /// </summary>

    // Recipes
    public GameObject recipesPanel;
    // Recipe
    public GameObject recipePanel;
    public Image recipePageImage;

    // copy mode
    private bool copyMode = false;
    // navigator
    public List<TextMeshPro> list_JoystickAxisText;

    private void Start()
    {
        StartCoroutine(InitSetting());
    }
    IEnumerator InitSetting()
    {
        initState = CMDState.BUSY;

        ToggleUIObjects();
        yield return new WaitUntil(() => ((DataManager.instance.initState == CMDState.IDLE) &
                                          (CheckPalletManagerInitStateReady()) == true));

        //Debug.Log("EditorUIManager : checked DataManager and Pallet init finihsed ");
        //ToggleUIObjects();
        yield return new WaitUntil(() => coroutine == null);

        DeactiveUIObjectsAfterCubeDesignDestroyed();
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        screenOrientation = Screen.orientation;

        initState = CMDState.IDLE;
        //Debug.Log("Editor UI Manager : InitSetting() completed !!");

    }
    private bool CheckPalletManagerInitStateReady()
    {
        /*Debug.LogWarning(DataManager.instance.initState);
        Debug.LogWarning(PalletManagerForPro.Instance == null);
        if(PalletManagerForPro.Instance != null)
            Debug.LogWarning(PalletManagerForPro.Instance.initState);*/

        if ((GameManager.instance._mode == e_Mode.Kids) & (PalletManager.Instance != null))
            if (PalletManager.Instance.initState == CMDState.IDLE) return true;

        if ((GameManager.instance._mode == e_Mode.Pro) & (PalletManagerForPro.Instance != null))
            if (PalletManagerForPro.Instance.initState == CMDState.IDLE) return true;

        return false;
    }
    private void ActiveUIObjectsAfterCubeDesignInstatiated()
    {
        foreach (var obj in objectsWhatYouWantToOnOffDependOnCubeDesign)
        {
            if (obj != null) obj.SetActive(true);
        }
    }
    private void DeactiveUIObjectsAfterCubeDesignDestroyed()
    {
        foreach (var obj in objectsWhatYouWantToOnOffDependOnCubeDesign)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void ToggleUIObjects()
    {
        foreach (var obj in objectsWhatYouWantToRefresh)
        {
            if (obj != null) obj.SetActive(!obj.activeSelf);
        }
    }
    private void ActiveUIObjects()
    {
        foreach (var obj in objectsWhatYouWantToRefresh)
        {
            if (obj != null) obj.SetActive(obj.activeSelf ? false : true);
        }
    }

    private void DeactiveUIObjects()
    {
        foreach (var obj in objectsWhatYouWantToRefresh)
        {
            if (obj != null) obj.SetActive(obj.activeSelf ? true : false);
        }
    }
    private void Update()
    {
#if UNITY_IOS || UNITY_ANDROID
        if ((Input.deviceOrientation == DeviceOrientation.Portrait) |
           (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown))
        {
            screenOrientation = ScreenOrientation.Portrait;
        }
        else if ((Input.deviceOrientation == DeviceOrientation.LandscapeLeft) |
                (Input.deviceOrientation == DeviceOrientation.LandscapeRight))
        {
            screenOrientation = ScreenOrientation.Landscape;
        }

        /*//when screen orientation is changed
        if (((Screen.orientation == ScreenOrientation.Landscape) |
            (Screen.orientation == ScreenOrientation.LandscapeLeft) |
            (Screen.orientation == ScreenOrientation.LandscapeRight)) &
            (screenOrientation != ScreenOrientation.Landscape))
        {
            brickListDropDownForPortrait.SetActive(false);
            brickListDropDownForLandscape.SetActive(true);
            screenOrientation = ScreenOrientation.Landscape;
        }
        else if (((Screen.orientation == ScreenOrientation.Portrait) |
                 (Screen.orientation == ScreenOrientation.PortraitUpsideDown)) &
                 (screenOrientation != ScreenOrientation.Portrait))
        {
            brickListDropDownForLandscape.SetActive(false);
            brickListDropDownForPortrait.SetActive(true);
            screenOrientation = ScreenOrientation.Portrait;
        }*/
#endif
        // check key input for PC environment.
        if (Input.GetKeyDown(KeyCode.A))
            MoveNodeCurser_Xminus();
        if (Input.GetKeyDown(KeyCode.E))
            MoveNodeCurser_Xplus();
        if (Input.GetKeyDown(KeyCode.W))
            MoveNodeCurser_Yplus();
        if (Input.GetKeyDown(KeyCode.S))
            MoveNodeCurser_Yminus();
        if (Input.GetKeyDown(KeyCode.D))
            MoveNodeCurser_Zminus();
        if (Input.GetKeyDown(KeyCode.Q))
            MoveNodeCurser_Zplus();

        if (Input.GetKeyDown(KeyCode.Insert))
            PutButtonOnClick();
        if (Input.GetKeyDown(KeyCode.Delete))
            DeleteButtonOnClick();
    }

    public void SelectResolution(int inputResolution)
    {
        if(inputResolution == 0)
            inputResolution = 10;
        CreateCubeDesign(inputResolution);
    }

    // Create cubedesign with type input
    public void CreateCubeDesign(int inputResolution)
    {
        if (cubeDesigneInstance != null)
        {
            CubeDesignGrid.instance.ToggleGrid("XYZ");
            DestroyCubeDesigne();
        }
        cubeDesigneInstance = (CubeDesign)Instantiate(cubeDesign, whereYouWantToCreateCubeDesign);
        DataManager.instance.SetCurrentCubeDesign(cubeDesigneInstance);
        cubeDesigneInstance.SelectPixel(inputResolution);
        CameraHandler.instance.SetCameraDefaultSettings();
        CubeDesignGrid.instance.ToggleGrid("XYZ");
        BrickSelectionManager.instance.Refresh();
        ActiveUIObjectsAfterCubeDesignInstatiated();
    }
    // Create cubedesign with type remembered.
    public void CreateCubeDesign()
    {
        if (cubeDesigneInstance != null)
        {
            CubeDesignGrid.instance.ToggleGrid("XYZ");
            DestroyCubeDesigne();
        }
        cubeDesigneInstance = (CubeDesign)Instantiate(cubeDesign, whereYouWantToCreateCubeDesign);
        DataManager.instance.SetCurrentCubeDesign(cubeDesigneInstance);
        cubeDesigneInstance.SelectPixel(resolution);
        CameraHandler.instance.SetCameraDefaultSettings();
        CubeDesignGrid.instance.ToggleGrid("XYZ");
        BrickSelectionManager.instance.Refresh();
        ActiveUIObjectsAfterCubeDesignInstatiated();
    }
    // Destroy CubeDesign
    public void DestroyCubeDesigne()
    {
        cubeDesigneInstance.DestroyAllEntities();
        //cubeDesigneInstance.DestroyThis();
        Destroy(cubeDesigneInstance.gameObject);
    }

    public CubeDesign GetCurrentCubeDesign()
    {
        return cubeDesigneInstance;
    }

    // when user push "put" button, register the design to inventory
    public void PutButtonOnClick()
    {
        //cubeDesigneInstance.PutBrick();
        cubeDesigneInstance.OnPutClicked();
    }
    public void DeleteButtonOnClick()
    {
        //cubeDesigneInstance.DeleteBrick();
        cubeDesigneInstance.OnDeleteClicked();
    }


    /// <summary>
    /// UI Managing
    /// </summary>

    IEnumerator ActiveUIObjectWithScalingUp(GameObject target, e_RectPositionType rectPositionType, float scailingTime, float frameNum)
    {
        float scale = 0f;
        float width = 0f;
        float height = 0f;
        target.SetActive(true);
        target.transform.localScale = new Vector3(scale, scale, scale);
        width = target.GetComponent<RectTransform>().sizeDelta.x;
        height = target.GetComponent<RectTransform>().sizeDelta.y;
        Vector2 posVec2;
        Vector2 posVec2MoveUnit;
        switch (rectPositionType)
        {
            case e_RectPositionType.Middle:
                posVec2 = new Vector2(0f, 0f);
                break;
            case e_RectPositionType.LeftTop:
                posVec2 = new Vector2(width / 2, -height / 2);
                break;
            case e_RectPositionType.RightTop:
                posVec2 = new Vector2(-width / 2, -height / 2);
                break;
            case e_RectPositionType.LeftBottom:
                posVec2 = new Vector2(width / 2, height / 2);
                break;
            case e_RectPositionType.RightBottom:
                posVec2 = new Vector2(-width / 2, height / 2);
                break;
            default:
                posVec2 = new Vector2(0f, 0f);
                break;
        }
        posVec2MoveUnit = new Vector2(posVec2.x / frameNum,
                                         posVec2.y / frameNum);
        target.transform.Translate(-posVec2);

        while (scale < 1.0f)
        {
            scale += 1 / frameNum / scailingTime;
            target.transform.localScale = new Vector3(scale, scale, scale);
            target.transform.Translate(posVec2MoveUnit / scailingTime);
            yield return new WaitForSeconds(1 / frameNum);
        }
        yield return null;

        coroutine = null;
    }
    IEnumerator DeactiveUIObjectWithScalingDown(GameObject target, e_RectPositionType rectPositionType, float scailingTime, float frameNum)
    {
        float scale = 1f;
        float width = 0f;
        float height = 0f;
        target.transform.localScale = new Vector3(scale, scale, scale);
        width = target.GetComponent<RectTransform>().sizeDelta.x;
        height = target.GetComponent<RectTransform>().sizeDelta.y;
        Vector2 posVec2;
        Vector2 posVec2MoveUnit;
        switch (rectPositionType)
        {
            case e_RectPositionType.Middle:
                posVec2 = new Vector2(0f, 0f);
                break;
            case e_RectPositionType.LeftTop:
                posVec2 = new Vector2(width / 2, -height / 2);
                break;
            case e_RectPositionType.RightTop:
                posVec2 = new Vector2(-width / 2, -height / 2);
                break;
            case e_RectPositionType.LeftBottom:
                posVec2 = new Vector2(width / 2, height / 2);
                break;
            case e_RectPositionType.RightBottom:
                posVec2 = new Vector2(-width / 2, height / 2);
                break;
            default:
                posVec2 = new Vector2(0f, 0f);
                break;
        }
        posVec2MoveUnit = new Vector2(posVec2.x / frameNum,
                                         posVec2.y / frameNum);

        while (scale > 0.0f)
        {
            scale -= 1 / frameNum / scailingTime;
            target.transform.localScale = new Vector3(scale, scale, scale);
            target.transform.Translate(-posVec2MoveUnit / scailingTime);
            yield return new WaitForSeconds(1 / frameNum);
        }
        target.SetActive(false);
        target.transform.Translate(posVec2);
        yield return null;

        coroutine = null;
    }
    // Moves ui object 
    IEnumerator MoveUIObject(GameObject targetObj, float distance, e_RectPositionType direction, float time)
    {
        float unitDistance;
        Vector3 unitVec;
        Vector3 targetVec;
        Vector3 remainVec;

        switch (direction)
        {
            case e_RectPositionType.Middle:
                targetVec = new Vector3(0f, 0f, 0f);
                break;
            case e_RectPositionType.LeftTop:
                targetVec = new Vector3(-distance, -distance, 0f);
                break;
            case e_RectPositionType.RightTop:
                targetVec = new Vector3(distance, distance, 0f);
                break;
            case e_RectPositionType.LeftBottom:
                targetVec = new Vector3(-distance, -distance, 0f);
                break;
            case e_RectPositionType.RightBottom:
                targetVec = new Vector3(distance, -distance, 0f);
                break;
            case e_RectPositionType.Top:
                targetVec = new Vector3(0f, distance, 0f);
                break;
            case e_RectPositionType.Bottom:
                targetVec = new Vector3(0f, -distance, 0f);
                break;
            case e_RectPositionType.Left:
                targetVec = new Vector3(-distance, 0f, 0f);
                break;
            case e_RectPositionType.Right:
                targetVec = new Vector3(distance, 0f, 0f);
                break;
            default:
                targetVec = new Vector3(0f, 0f, 0f);
                break;
        }
        remainVec = targetVec;
        unitDistance = distance * Time.deltaTime;
        unitVec = targetVec * Time.deltaTime;
        while ((Mathf.Abs(remainVec.x) > 2 * unitDistance))
        {
            targetObj.transform.Translate(unitVec, Space.Self);
            remainVec -= unitVec;
            yield return new WaitForSeconds(Time.deltaTime * time);
        }
        targetObj.transform.Translate(remainVec, Space.Self);
        yield return null;
    }


    //Cube Designe methods interface
    //========================================================================================

    // cube design cursor
    public void MoveNodeCurser_Xplus() { cubeDesigneInstance.MoveCursor_Xplus(); }
    public void MoveNodeCurser_Xminus() { cubeDesigneInstance.MoveCursor_Xminus(); }
    public void MoveNodeCurser_Yplus() { cubeDesigneInstance.MoveCursor_Yplus(); }
    public void MoveNodeCurser_Yminus() { cubeDesigneInstance.MoveCursor_Yminus(); }
    public void MoveNodeCurser_Zplus() { cubeDesigneInstance.MoveCursor_Zplus(); }
    public void MoveNodeCurser_Zminus() { cubeDesigneInstance.MoveCursor_Zminus(); }
    public void MoveNodeCurserTo(int i) { cubeDesigneInstance.MoveCursorTo(i); }

    public void RotateNode_X() { cubeDesigneInstance.RotateCusor_X(); }
    public void RotateNode_Y() { cubeDesigneInstance.RotateCusor_Y(); }
    public void RotateNode_Z() { cubeDesigneInstance.RotateCusor_Z(); }
    public void ScaleUpBrick() { cubeDesigneInstance.ScaleUpBrick(); }

    // node type selection
    public void SelectBrickMesh(Mesh mesh)
    {
        cubeDesigneInstance.SelectBrick(mesh);
    }

    ///
    /// cube design modes 
    ///

    // auto put
    public void ToggleAutoPut()
    {
        if (IsCubeDesignInstanceExist() == false) return;

        if (cubeDesigneInstance.autoPut == true)
        {
            cubeDesigneInstance.SetAutoPut(false);
        }
        else
        {
            cubeDesigneInstance.SetAutoPut(true);
        }
    }
    // auto delete
    public void ToggleAutoDelete()
    {
        if (IsCubeDesignInstanceExist() == false) return;

        if (cubeDesigneInstance.autoDelete == true)
        {
            cubeDesigneInstance.SetAutoDelete(false);
        }
        else
        {
            cubeDesigneInstance.SetAutoDelete(true);
        }
    }

    // select
    public void OnSelectClicked()
    {
        if (IsCubeDesignInstanceExist() == false) return;

        int id = cubeDesigneInstance.GetCursorID();
        if (cubeDesigneInstance.IsBrickExistOnIndex(id) == true)
        {
            BrickSelectionManager.instance.Select(id);
        }
        else
        {
            Debug.Log("You tried to select brick but brick does not exist !");
        }
    }

    public void OnSelectAllClicked()
    {
        if (IsCubeDesignInstanceExist() == false) return;
        BrickSelectionManager.instance.SelectAll();
    }
    private bool IsCubeDesignInstanceExist()
    {
        bool isExist = false;
        if (cubeDesigneInstance == null)
        {
            Debug.Log("cube design wasn't instatiated");
        }
        else
        {
            isExist = true;
        }

        return isExist;
    }

    // Select Mode Toggle
    public void ToggleCopyMode()
    {
        if (copyMode == true)
        {
            copyMode = false;
        }
        else
        {
            copyMode = true;
        }
    }

    // joystick axis text
    public void ToggleJoystickAxisTexts()
    {
        foreach (var textMeshPro in list_JoystickAxisText)
        {
            GameObject obj = textMeshPro.gameObject;
            obj.SetActive(obj.activeSelf ? false : true);
        }
    }
}