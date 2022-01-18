using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GoogleMobileAds.Api;
public class AdmobManager : MonoBehaviour
{
    public static AdmobManager instance;
    public static CMDState CMDState = CMDState.BUSY;
    private void Awake()
    {
        if (instance == null) instance = this;
        DontDestroyOnLoad(instance);
    }

    public bool isTest;

    private void Start()
    {
        var requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(new List<string>() { "05157df51a731d3b" }) // test deevice list
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.Initialize(initStatus => { CMDState = CMDState.IDLE; });
    }
}

