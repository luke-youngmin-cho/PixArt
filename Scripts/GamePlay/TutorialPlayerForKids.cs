using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum e_TutorialStep
{
    SelectSize,
    SelectBG,
    FinishInitSettings,
    OpenBrickPanel,
    SelectBrickCube,
    CloseBrickPanel,
    ChooseColor,
    OpenColorPicker,
    ChangeColor,
    ExplainJoystick,
    DragJoystick,
    DragJoystickFinish,
    ClickJoystick,
    PressJoystick,
    ExplainJoystickFinish,
    PutBrick,
    PutBrickFinish,
    DelBrick,
    ClickSaveButton,
    Save,
    ClickOpenButton,
    CloseOpenPanel,
    Finished,
}
public class TutorialPlayerForKids : MonoBehaviour
{
    [SerializeField] GameObject[] tutorialGuides;
    e_TutorialStep step;
    Coroutine coroutineJoystick;
    Coroutine coroutineTimeout;

    /// <summary>
    /// Access
    /// </summary>
    [SerializeField] Material skyboxMat;
    [SerializeField] Toggle brickListToggle;
    [SerializeField] GameObject brickListPanel;
    [SerializeField] Mesh cubeMesh;
    [SerializeField] GameObject cubeForPractice1;
    [SerializeField] GameObject cubeForPractice2;
    [SerializeField] GameObject cubeForPractice3;
    [SerializeField] GameObject userInputToSaveDataPanel;
    [SerializeField] OpenPanel openPanel;
    [SerializeField] bl_SceneLoader sceneLoader;
    void Start()
    {
        tutorialGuides[0].SetActive(true);
    }
    // Workflow
    //=========================================================================================================
    private void Update()
    {
        Workflow();
    }

    private void Workflow()
    {
        switch (step)
        {
            case e_TutorialStep.SelectSize:
                if (CheckSize5x5x5() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.SelectBG:
                if (CheckBG() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.FinishInitSettings:
                if (CheckInitSettingsFinished() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.OpenBrickPanel:
                if (CheckBrickListPanelOpened() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.SelectBrickCube:
                //next button implemented
                break;
            case e_TutorialStep.CloseBrickPanel:
                if (CheckBrickListPanelClosed() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.ChooseColor:
                if (CheckColorChosen() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.OpenColorPicker:
                if (CheckColorPickerOpened() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.ChangeColor:
                if (CheckColorPickerClosed() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.ExplainJoystick:
                //next button implemeted
                break;
            case e_TutorialStep.DragJoystick:
                //next button implemeted
                break;
            case e_TutorialStep.DragJoystickFinish:
                //next button implemeted
                break;
            case e_TutorialStep.ClickJoystick:
                //next button implemeted
                break;
            case e_TutorialStep.PressJoystick:
                //next button implemeted
                break;
            case e_TutorialStep.PutBrick:

                if (CheckPutBrickRightPlace() == true)
                {
                    Destroy(cubeForPractice1);
                    Destroy(cubeForPractice2);
                    Destroy(cubeForPractice3);
                    ConstructingPanelManager.instance.OnPutButtonUp();
                    GoToTheNextStep();
                }
                if (cubeForPractice1.activeSelf == false)
                    cubeForPractice1.SetActive(true);
                if (cubeForPractice2.activeSelf == false)
                    cubeForPractice2.SetActive(true);
                if (cubeForPractice3.activeSelf == false)
                    cubeForPractice3.SetActive(true);
                break;
            case e_TutorialStep.PutBrickFinish:
                // next button implemented;
                break;
            case e_TutorialStep.DelBrick:
                if (CheckAllBricksAreDeleted() == true)
                {
                    ConstructingPanelManager.instance.OnDeleteButtonUp();
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.ClickSaveButton:
                if (CheckUserInputToSaveDataPanelOpened() == true)
                    GoToTheNextStep();
                break;
            case e_TutorialStep.Save:
                if (CheckUserInputToSaveDataPanelClosed() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.ClickOpenButton:
                if (CheckOpenPanelOpened() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.CloseOpenPanel:
                if (CheckOpenPanelClosed() == true)
                {
                    GoToTheNextStep();
                }
                break;
            case e_TutorialStep.Finished:
                SettingsManager.instance.Play_Tutorial = 0;
                break;
            default:
                break;
        }
    }
    private void closeAllGuides()
    {
        for (int i = 0; i < tutorialGuides.Length; i++)
        {
            tutorialGuides[i].SetActive(false);
        }
    }
    private void ResetAllCoroutines()
    {
        if (coroutineJoystick != null)
        {
            StopCoroutine(coroutineJoystick);
            coroutineJoystick = null;
        }
        if (coroutineTimeout != null)
        {
            StopCoroutine(coroutineTimeout);
            coroutineTimeout = null;
        }
    }


    //=========================================================================================
    // Guides functions
    //=========================================================================================

    // on mouse event
    //-----------------------------------------------------------------------------------------
    public void GoToTheNextStep()
    {
        tutorialGuides[(int)step].SetActive(false);
        step += 1;
        tutorialGuides[(int)step].SetActive(true);
    }
    public void SelectSize5x5x5()
    {
        InitPanel.instance.OnSelectResolutionButtonClicked(5);
        InitPanel.instance.SetSizeCursor(0);
    }

    public void SeleectBG()
    {
        InitPanel.instance.OnBGButtonClicked_Sky(skyboxMat);
        InitPanel.instance.SetBGCursor(6);
    }
    public void FinishInitSettings()
    {
        InitPanel.instance.OnOKButtonClicked();
    }
    public void OpenBrickPanel()
    {
        brickListToggle.isOn = true;
    }
    public void SelectBrickCube()
    {
        EditorUIManager.Instance.SelectBrickMesh(cubeMesh);
    }
    public void CloseBrickPanel()
    {
        brickListToggle.isOn = false;
    }
    public void ChooseColor()
    {
        PalletManager.Instance.SetSelectButtonIdx(1);
    }
    public void OpenColorPicker()
    {
        PalletManager.Instance.OnColorEditButtonClicked();
    }
    public void ClickColorPickerOK()
    {
        PalletManager.Instance.OnDoneButtonClicked();
    }
    public void JoystickXplus_PointerDown()
    {
        EditorUIManager.Instance.MoveNodeCurser_Xplus();
        if (coroutineJoystick == null)
            coroutineJoystick = StartCoroutine(CheckJoystickPressed("x+"));
    }
    public void JoystickXplus_PointerClick()
    {
        EditorUIManager.Instance.MoveNodeCurser_Xplus();
    }
    public void JoystickXminus_PointerDown()
    {
        EditorUIManager.Instance.MoveNodeCurser_Xminus();
        if (coroutineJoystick == null)
            coroutineJoystick = StartCoroutine(CheckJoystickPressed("x-"));
    }
    public void JoystickYplus_PointerDown()
    {
        EditorUIManager.Instance.MoveNodeCurser_Yplus();
        if (coroutineJoystick == null)
            coroutineJoystick = StartCoroutine(CheckJoystickPressed("y+"));
    }
    public void JoystickYminus_PointerDown()
    {
        EditorUIManager.Instance.MoveNodeCurser_Yminus();
        if (coroutineJoystick == null)
            coroutineJoystick = StartCoroutine(CheckJoystickPressed("y-"));
    }
    public void JoystickZplus_PointerDown()
    {
        EditorUIManager.Instance.MoveNodeCurser_Zplus();
        if (coroutineJoystick == null)
            coroutineJoystick = StartCoroutine(CheckJoystickPressed("z+"));
    }
    public void JoystickZminus_PointerDown()
    {
        EditorUIManager.Instance.MoveNodeCurser_Zminus();
        if (coroutineJoystick == null)
            coroutineJoystick = StartCoroutine(CheckJoystickPressed("z-"));
    }
    public void JoystickXplus_PointerUp()
    {
        StopCoroutine(coroutineJoystick);
        coroutineJoystick = null;
    }
    public void JoystickXminus_PointerUp()
    {
        StopCoroutine(coroutineJoystick);
        coroutineJoystick = null;
    }
    public void JoystickYplus_PointerUp()
    {
        StopCoroutine(coroutineJoystick);
        coroutineJoystick = null;
    }
    public void JoystickYminus_PointerUp()
    {
        StopCoroutine(coroutineJoystick);
        coroutineJoystick = null;
    }
    public void JoystickZplus_PointerUp()
    {
        StopCoroutine(coroutineJoystick);
        coroutineJoystick = null;
    }
    public void JoystickZminus_PointerUp()
    {
        StopCoroutine(coroutineJoystick);
        coroutineJoystick = null;
    }
    IEnumerator CheckJoystickPressed(string direction)
    {
        yield return new WaitForSeconds(10.0f * Time.deltaTime);
        while (true)
        {
            switch (direction)
            {
                case "x+":
                    EditorUIManager.Instance.MoveNodeCurser_Xplus();
                    break;
                case "x-":
                    EditorUIManager.Instance.MoveNodeCurser_Xminus();
                    break;
                case "y+":
                    EditorUIManager.Instance.MoveNodeCurser_Yplus();
                    break;
                case "y-":
                    EditorUIManager.Instance.MoveNodeCurser_Yminus();
                    break;
                case "z+":
                    EditorUIManager.Instance.MoveNodeCurser_Zplus();
                    break;
                case "z-":
                    EditorUIManager.Instance.MoveNodeCurser_Zminus();
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(2.0f * Time.deltaTime);
        }
    }
    public void OnSaveButtonClick()
    {
        userInputToSaveDataPanel.SetActive(true);
    }
    public void OnOpenButtonClick()
    {
        openPanel.OpenThis();
    }
    public void CloseOpenPanel()
    {
        openPanel.CloseThis();
    }
    public void StartNormalMode()
    {
        StopAllCoroutines();
        tutorialGuides[(int)e_TutorialStep.Finished].SetActive(false);
        this.gameObject.SetActive(false);
        sceneLoader.LoadLevel("CubeEditorForKids");
    }
    // checking step state
    //-----------------------------------------------------------------------------------------
    private bool CheckSize5x5x5()
    {
        bool isOK = false;
        if (InitPanel.instance.GetResolution() == 5)
            isOK = true;
        return isOK;
    }
    private bool CheckBG()
    {
        bool isOK = false;
        if (InitPanel.instance.GetBGCursorIdx() == 6)
            isOK = true;
        return isOK;
    }
    private bool CheckInitSettingsFinished()
    {
        bool isOK = false;
        if (EditorUIManager.Instance.cubeDesigneInstance != null)
            isOK = true;
        return isOK;
    }
    private bool CheckBrickListPanelOpened()
    {
        bool isOK = false;
        if (brickListPanel.activeSelf == true)
            isOK = true;
        return isOK;
    }
    private bool CheckCubeBrickSelected()
    {
        bool isOK = false;
        if (EditorUIManager.Instance.cubeDesigneInstance.GetCursorInfo()._meshName == "Cube")
            isOK = true;
        return isOK;
    }
    private bool CheckBrickListPanelClosed()
    {
        bool isOK = false;
        if (brickListPanel.activeSelf == false)
            isOK = true;
        return isOK;
    }
    private bool CheckColorChosen()
    {
        bool isOK = false;
        if (PalletManager.Instance.selectedMaterial != null)
        {
            if (PalletManager.Instance.selectedMaterial.name == "PalletSelectButtonMaterialForKids 1")
                isOK = true;
        }
        return isOK;
    }
    private bool CheckColorPickerOpened()
    {
        bool isOK = false;
        if (PalletManager.Instance.isColorPickerOn() == true)
            isOK = true;
        return isOK;
    }
    private bool CheckColorPickerClosed()
    {
        bool isOK = false;
        if (PalletManager.Instance.isColorPickerOn() == false)
            isOK = true;
        return isOK;
    }

    IEnumerator Timeout(float time)
    {
        yield return new WaitForSeconds(time * Time.deltaTime);
        StopCoroutine(coroutineTimeout);
        coroutineTimeout = null;
    }
    private bool CheckPutBrickRightPlace()
    {
        bool isOK = false;
        if ((EditorUIManager.Instance.cubeDesigneInstance.GetBrickInfo(4)._isExist == true) &
            (EditorUIManager.Instance.cubeDesigneInstance.GetBrickInfo(24)._isExist == true) &
            (EditorUIManager.Instance.cubeDesigneInstance.GetBrickInfo(100)._isExist == true))
            isOK = true;
        return isOK;
    }
    private bool CheckAllBricksAreDeleted()
    {
        bool isOK = false;
        if (EditorUIManager.Instance.cubeDesigneInstance.totalBrickNumber == 0)
            isOK = true;
        return isOK;
    }
    private bool CheckUserInputToSaveDataPanelOpened()
    {
        bool isOK = false;
        if (userInputToSaveDataPanel.activeSelf == true)
            isOK = true;
        return isOK;
    }
    private bool CheckUserInputToSaveDataPanelClosed()
    {
        bool isOK = false;
        if (userInputToSaveDataPanel.activeSelf == false)
            isOK = true;
        return isOK;
    }
    private bool CheckOpenPanelOpened()
    {
        bool isOK = false;
        if (openPanel.gameObject.activeSelf == true)
            isOK = true;
        return isOK;
    }
    private bool CheckOpenPanelClosed()
    {
        bool isOK = false;
        if (openPanel.gameObject.activeSelf == false)
            isOK = true;
        return isOK;
    }
}