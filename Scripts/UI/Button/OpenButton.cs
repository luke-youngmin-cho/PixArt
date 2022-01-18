using DOTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenButton : MonoBehaviour
{
    [SerializeField] private GameObject doubleCheckingPanel;
    [SerializeField] private OpenPanel openPanel;
    public void OnButtonClicked()
    {
        CubeDesign cubeDesign = EditorUIManager.Instance.GetCurrentCubeDesign();

        if ((cubeDesign != null) & (cubeDesign._isSaved == true))
        {
            openPanel.OpenThis();
        }
        else
        {
            doubleCheckingPanel.SetActive(true);
        }
    }
}
