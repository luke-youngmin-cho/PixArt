using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PremiumMaskForToggle : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    
    private void OnEnable()
    {
        if(Purchaser.purchasedFlags == e_Purchased.Premium)
        {
            toggle.interactable = true;
        }
        else
        {
            toggle.interactable = false;
        }
    }
}
