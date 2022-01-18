using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class PalletColorData
{
    public List<float4> list_color;
}

[System.Serializable]
public class PalletData
{
    public int id; // 0 = default (use for save cube design.
    public PalletColorData palletColorData;

    public PalletData() { }
    public PalletData(bool doYouWantToGetCurrentPalletData)
    {
        if(doYouWantToGetCurrentPalletData == true)
        {
            switch (GameManager.instance._mode)
            {
                case e_Mode.Kids:
                    palletColorData.list_color = PalletManager.Instance.GetColorData();
                    break;
                case e_Mode.Pro:
                    palletColorData.list_color = PalletManagerForPro.Instance.GetColorData();
                    break;
                default:
                    break;
            }
        }
    }
}
[System.Serializable]
public class PalletsData
{
    public List<PalletData> palletsData;
    public PalletsData()
    {   
    }
}
