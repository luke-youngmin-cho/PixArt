using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheckAdRemovedAndActiveObj : MonoBehaviour
{
    [SerializeField] GameObject objToActive;
    [SerializeField] GameObject objToDestroy;
    void Start()
    {
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitUntil(() => Purchaser.CMDState == CMDState.IDLE);
        Purchaser.instance.RefreshPurchaseFlag();
        if ((Purchaser.purchasedFlags == e_Purchased.Premium)|
            (Purchaser.purchasedFlags == e_Purchased.RemovedAds))
        {
            objToActive.SetActive(true);
            Destroy(objToDestroy);
        }
    }
}
