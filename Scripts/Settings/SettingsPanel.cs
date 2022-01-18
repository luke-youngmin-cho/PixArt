using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Button tab1;
    public Button tab2;
    public Button tab3;
    public Button tab4;

    public GameObject tab1Panel;
    public GameObject tab2Panel;
    public GameObject tab3Panel;
    public GameObject tab4Panel;

    public void ActiveThis(){ this.gameObject.SetActive(true); }
    public void DeactiveThis(){ this.gameObject.SetActive(false); } 

    public void OnClickTab1()
    {
        DeactivaAllPanels();
        tab1Panel.SetActive(true);
    }
    public void OnClickTab2()
    {
        DeactivaAllPanels();
        tab2Panel.SetActive(true);
    }
    public void OnClickTab3()
    {
        DeactivaAllPanels();
        tab3Panel.SetActive(true);
    }
    public void OnClickTab4()
    {
        DeactivaAllPanels();
        tab4Panel.SetActive(true);
    }

    public void DeactivaAllPanels()
    {
        if (tab1Panel != null) tab1Panel.SetActive(false);
        if (tab2Panel != null) tab2Panel.SetActive(false);
        if (tab3Panel != null) tab3Panel.SetActive(false);
        if (tab4Panel != null) tab4Panel.SetActive(false);
    }

}
