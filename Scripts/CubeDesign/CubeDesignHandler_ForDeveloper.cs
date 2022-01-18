using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DOTS;
public class CubeDesignHandler_ForDeveloper : MonoBehaviour
{
    private CubeDesign cubeDesign;
    [SerializeField] InputField y_Index;
    public void DisableAllBricksAndCursor()
    {
        cubeDesign = EditorUIManager.Instance.GetCurrentCubeDesign();

        cubeDesign.DisableAllBricks();
        cubeDesign.DisableCursor();
        cubeDesign.MoveCursorTo(0);
        CubeDesignGrid.instance.GridOff();
        CubeDesignGrid.instance.GridOn(e_GridType.XZ);
    }
    public void EnableAllBricksAndCursor()
    {
        cubeDesign = EditorUIManager.Instance.GetCurrentCubeDesign();

        cubeDesign.EnableAllBricks();
        cubeDesign.EnableCursor();
        cubeDesign.MoveCursorTo(0);
    }
    public void EnableBricksFloor()
    {
        cubeDesign = EditorUIManager.Instance.GetCurrentCubeDesign();
        int resolution = cubeDesign.resolution;
        int inputY = int.Parse(y_Index.text);

        if (inputY >= resolution) return;
        cubeDesign.MoveCursorTo(inputY * resolution);
        cubeDesign.EnableBricksOnXZPlane(inputY);
    }
}
