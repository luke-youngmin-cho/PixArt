using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectForButton : MonoBehaviour
{
    public GameObject obj;

    public void ToggleObj()
    {
        if(obj.activeSelf == true)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
        }
    }
}
