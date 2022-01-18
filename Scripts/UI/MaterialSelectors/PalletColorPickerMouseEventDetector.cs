using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletColorPickerMouseEventDetector : MonoBehaviour
{
    public static PalletColorPickerMouseEventDetector instance;
    private void Awake()
    {
        instance = this;
        instance.gameObject.SetActive(false);
    }
        
}
