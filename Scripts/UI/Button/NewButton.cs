using DOTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewButton : MonoBehaviour
{
    [SerializeField] private GameObject doubleCheckingPanel;
    [SerializeField] private GameObject resolutionInputPanel;
    public void OnButtonClicked()
    {
        CubeDesign cubeDesign = EditorUIManager.Instance.GetCurrentCubeDesign();

        if ((cubeDesign == null) |
            ((cubeDesign !=null) & ((cubeDesign._isSaved == true))))
        {
            resolutionInputPanel.SetActive(true);
            return;
        }
        else
        {
            doubleCheckingPanel.SetActive(true);
        }
    }
}
