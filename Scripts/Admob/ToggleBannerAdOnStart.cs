using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBannerAdOnStart : MonoBehaviour
{
    public bool OnOff;
    void Start()
    {
        StartCoroutine(InitCoroutine());
    }
    IEnumerator InitCoroutine()
    {
        yield return new WaitUntil(() => (AdmobManager.CMDState == CMDState.IDLE) &
                                         (Admob_Banner.instance.CMDState == CMDState.IDLE));

        yield return new WaitUntil(() => Purchaser.CMDState == CMDState.IDLE);
        Purchaser.instance.RefreshPurchaseFlag();
        if ((Purchaser.purchasedFlags == e_Purchased.Premium) |
            (Purchaser.purchasedFlags == e_Purchased.RemovedAds))
        {
            Admob_Banner.instance.ToggleBannerView(false);
        }
        else
        {
            Admob_Banner.instance.ToggleBannerView(OnOff);
        }
    }
}
