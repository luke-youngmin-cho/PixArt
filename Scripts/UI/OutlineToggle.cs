using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DOTS;
public class OutlineToggle : MonoBehaviour
{
    public static OutlineToggle instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public Toggle toggle;

    public bool isOn;
    public GameObject outline;
    public void OutlineOnOff()
    {
        isOn = toggle.isOn;
        if (toggle.isOn == false)
        {
            outline.SetActive(true);
        }
        else
        {
            outline.SetActive(false);
        }
    }
}
