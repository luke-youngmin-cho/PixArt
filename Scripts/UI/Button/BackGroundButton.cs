using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BackGroundButton : MonoBehaviour
{
    public void ChangeBackground(string colorHex)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#"+colorHex, out color);
        CameraHandler.instance.ChangeClearFlag("solidcolor");
        CameraHandler.instance.ChangeCameraColor(color);
    }
    public void ChangeBackGround(Material skyMat)
    {
        CameraHandler.instance.ChangeClearFlag("skybox");
        CameraHandler.instance.ChangeSkyBox(skyMat);
    }

}
