using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulbHandlingWindow : MonoBehaviour
{
    private void OnEnable()
    {
        if (CameraHandler.instance != null)
            CameraHandler.instance.DisableHandling();
    }
    private void OnDisable()
    {
        if (CameraHandler.instance != null)
            CameraHandler.instance.EnableHandling();
    }
}
