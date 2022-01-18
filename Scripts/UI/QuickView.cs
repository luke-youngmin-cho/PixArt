using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickView : MonoBehaviour
{
    public Transform cubeDesignerTransform;
    public Transform navigator;
    public void QuickViewFree()
    {
        cubeDesignerTransform.eulerAngles = new Vector3(0f, -45f,0f);
        navigator.eulerAngles = cubeDesignerTransform.eulerAngles;
    }
    public void QuickViewXY()
    {
        cubeDesignerTransform.eulerAngles = new Vector3(0f,0,0f);
        navigator.eulerAngles = cubeDesignerTransform.eulerAngles;
    }
    public void QuickViewXZ()
    {
        cubeDesignerTransform.eulerAngles = new Vector3(-90, 0f, 0f);
        navigator.eulerAngles = cubeDesignerTransform.eulerAngles;
    }
    public void QuickViewYZ()
    {
        cubeDesignerTransform.eulerAngles = new Vector3(0f, 0, 90f);
        navigator.eulerAngles = cubeDesignerTransform.eulerAngles;
    }


}
