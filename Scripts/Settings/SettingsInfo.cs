using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsInfo : MonoBehaviour
{
    static public SettingsInfo instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }
}
