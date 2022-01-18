using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [HideInInspector]public string _androidGameId = AdsData.Android_App_ID;
    [HideInInspector]public string _iOsGameId = AdsData.IOS_App_ID;
    [SerializeField] bool _testMode = true;
    [SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId;
    private bool initializationFinished = false;
    void Awake()
    {
        var requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(new List<string>() { "05157df51a731d3b" }) // test deevice list
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);
        InitializeAds();
    }
    private void Start()
    {
        StartCoroutine(LoadMainSceneAfterAdsInitializationFinished());
    }
    IEnumerator LoadMainSceneAfterAdsInitializationFinished()
    {
        yield return null;
        yield return new WaitUntil(() => initializationFinished == true);
        SceneManager.LoadScene("Main");
    }
    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        if (Advertisement.isSupported && !Advertisement.isInitialized)
        {
            Advertisement.Initialize(_gameId, _testMode, _enablePerPlacementMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        initializationFinished = true;
    }
    
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

}