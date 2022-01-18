using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
public class Admob_Banner : MonoBehaviour
{
    // Banner
    public static Admob_Banner instance;
    [HideInInspector] public CMDState CMDState = CMDState.BUSY;

    private const string AD_BannerTest_ID = AdsData.AD_BannerTest_ID;
    [HideInInspector] public string AD_Banner_ID;
    private string ID;
    BannerView bannerView;
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }   
        AD_Banner_ID = AdsData.AD_Banner_Android_ID;
    }
    public virtual void Start()
    {
        ID = AdmobManager.instance.isTest ? AD_BannerTest_ID : AD_Banner_ID;
        StartCoroutine(InitCoroutine());
    }
    IEnumerator InitCoroutine()
    {
        yield return new WaitUntil(() => AdmobManager.CMDState == CMDState.IDLE);
        yield return new WaitUntil(() => Purchaser.CMDState == CMDState.IDLE);
        
        if (Purchaser.instance.IsAdRemoved())
        {
            // don't play ads
        }
        else
        {
            LoadAD_Banner();
            ToggleBannerView(true);
        }
        CMDState = CMDState.IDLE;
    }
    void LoadAD_Banner()
    {
        bannerView = new BannerView(ID,AdSize.Banner, AdPosition.Bottom);
        bannerView.LoadAd(GetAdRequest());
        ToggleBannerView(false);
    }
    public virtual void ToggleBannerView(bool isOn)
    {
        if (isOn) bannerView.Show();
        else bannerView.Hide();
    }

    AdRequest GetAdRequest()
    {
        return new AdRequest.Builder().Build();
    }
}
