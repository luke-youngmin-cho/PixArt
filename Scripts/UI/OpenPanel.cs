using DOTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenPanel : MonoBehaviour
{
    static public OpenPanel instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    [SerializeField] GameObject askUserReallyWantToDeleteThisDesignPanel;
    private Text tmpNameToBeDeleted;
    public void OpenThis()
    {
        this.gameObject.SetActive(true);
        Debug.Log("Try to open designs list!");
        StartCoroutine(DataManager.instance.GetSavedDesignInfo());
    }
    public void CloseThis()
    {
        this.gameObject.SetActive(false);
        DataManager.instance.RemoveUIForSavedDesign();
    }
    public void Refresh()
    {
        if (isOpened())
        {
            CloseThis();
            OpenThis();
        }
    }
    public bool isOpened()
    {
        return gameObject.activeSelf;
    }

    public void PopUpDoubleCheckingPanelToDeleteData(Text name)
    {

        askUserReallyWantToDeleteThisDesignPanel.SetActive(true);
        tmpNameToBeDeleted = name;
        
    }
    public void DeleteCubeDesignData()
    {
        if (DataManager.instance == null) return;
        DataManager.instance.DeleteCubeDesignData(tmpNameToBeDeleted);
    }

    public void OpenCubeDesignData(Text name)
    {
        if (DataManager.instance == null) return;
        DataManager.instance.OpenCubeDesignData(name);
    }
}
