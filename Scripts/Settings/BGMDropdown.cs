using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum e_BGMDropdownType
{
    Random,
    Track1,
    Track2,
    Track3,
    Track4
}
public class BGMDropdown : MonoBehaviour
{
    static public BGMDropdown instance;
    private Dropdown thisDD;
    private void Awake()
    {
        if (instance == null) instance = this;
        thisDD = this.gameObject.GetComponent<Dropdown>();
    }
    public void SelectBGM()
    {
        int currentBGMTypeIndex = thisDD.value;        
        SettingsManager.instance.Sound_BGMTypeIndex = currentBGMTypeIndex;
        Debug.Log($"BGM {currentBGMTypeIndex} saved!");
    }
    public void SetDropdownValue(int value)
    {
        thisDD.value = value;
    }
}
