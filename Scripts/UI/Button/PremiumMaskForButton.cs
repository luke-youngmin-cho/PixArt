using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PremiumMaskForButton : MonoBehaviour
{
    [SerializeField] Button button;

    private void OnEnable()
    {
        if (Purchaser.purchasedFlags == e_Purchased.Premium)
        {
            button.interactable = true;
            this.gameObject.SetActive(false);
        }
        else
        {
            button.interactable = false;
        }
    }
}
