using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle_PlayTutorial : MonoBehaviour
{

    public void OnToggle(bool isOn)
    {
        if (SettingsManager.instance == null) return;

        SettingsManager.instance.Play_Tutorial = (isOn == true ? 1 : 0);
    }
}
