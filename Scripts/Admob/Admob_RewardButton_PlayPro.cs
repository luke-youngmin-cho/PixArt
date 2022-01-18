using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
public class Admob_RewardButton_PlayPro : Admob_RewardButton
{
    public GameManager gameManger;
    public bl_SceneLoader sceneLoader;
    public AudioManager audioManager;

    [SerializeField] GameObject internetConnectionWarningPopup;
    public override void Awake()
    {
        rewardID = AdsData.AD_reward_Android_ID_PlayPro;
    }
    public override void Start()
    {
        SetRewardEvent(RewardEventToPlayPro);
        base.Start();
    }
    private void RewardEventToPlayPro()
    {
        gameManger.SetMode("Pro");
        sceneLoader.LoadLevel("CubeEditorForPro");
        audioManager.StopAll();
    }
    private void FailedEventToPlayPro()
    {
        internetConnectionWarningPopup.SetActive(true);
    }
}
