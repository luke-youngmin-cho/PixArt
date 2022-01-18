using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ConstructingPanelManager : MonoBehaviour
{
    static public ConstructingPanelManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public GameObject putButton;
    public GameObject deleteButton;

    public bool isPanelUsing;
    private bool wasPutButtonClicked;
    private bool wasDeleteButtonClicked;

    e_PressFSM pressFSM;
    Vector2 localCursor;

    Coroutine pointerCoroutine;
    
    private void Update()
    {
        WorkFlow();
    }
    private void WorkFlow()
    {
        switch (pressFSM)
        {
            case e_PressFSM.Idle:
                break;
            case e_PressFSM.Clicked:
                CameraHandler.instance.DisableHandling();
                if (pointerCoroutine == null) pointerCoroutine = StartCoroutine(DelayToChangePressedState());
                break;
            case e_PressFSM.Presssed:
                Debug.Log("pressing!");
                break;
            case e_PressFSM.Detached:
                if (pointerCoroutine != null)
                {
                    StopCoroutine(pointerCoroutine);
                    pointerCoroutine = null;
                }
                CameraHandler.instance.EnableHandling();
                EditorUIManager.Instance.cubeDesigneInstance.autoPut    = false;
                EditorUIManager.Instance.cubeDesigneInstance.autoDelete = false;
                pressFSM = e_PressFSM.Idle;
                break;
            default:
                break;
        }
    }
    IEnumerator DelayToChangePressedState()
    {
        yield return new WaitForSeconds(0.05f);
        if (pressFSM == e_PressFSM.Clicked)
        {
            EditorUIManager.Instance.cubeDesigneInstance.autoPut = wasPutButtonClicked;
            EditorUIManager.Instance.cubeDesigneInstance.autoDelete = wasDeleteButtonClicked;
            pressFSM = e_PressFSM.Presssed;
        }
        else
        {
            pressFSM = e_PressFSM.Detached;
        }

        pointerCoroutine = null;
    }
    public void OnPutButtonDown()
    {
        EditorUIManager.Instance.PutButtonOnClick();
        wasPutButtonClicked = true;
        wasDeleteButtonClicked = false;
        pressFSM = e_PressFSM.Clicked;
    }
    public void OnPutButtonClick()
    {
        wasPutButtonClicked = false;
        wasDeleteButtonClicked = false;
        pressFSM = e_PressFSM.Detached;
    }
    public void OnPutButtonUp()
    {
        wasPutButtonClicked = false;
        wasDeleteButtonClicked = false;
        pressFSM = e_PressFSM.Detached;
    }
    public void OnDeleteButtonDown()
    {
        EditorUIManager.Instance.DeleteButtonOnClick();
        wasPutButtonClicked = false;
        wasDeleteButtonClicked = true;
        pressFSM = e_PressFSM.Clicked;
    }
    public void OnDeleteButtonClick()
    {
        wasPutButtonClicked = false;
        wasDeleteButtonClicked = false;
        pressFSM = e_PressFSM.Detached;
    }
    public void OnDeleteButtonUp()
    {
        wasPutButtonClicked = false;
        wasDeleteButtonClicked = false;
        pressFSM = e_PressFSM.Detached;
    }
}
