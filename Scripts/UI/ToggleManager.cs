using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public List<Toggle> toggles;
    public bool refreshInAwake;
    public bool refreshWithCoroutine;
    public delegate void refreshFunc();
    public int togglesNum { get { return toggles.Count; } }

    public virtual void Start()
    {
        if (refreshInAwake == true)
        {
            if (refreshWithCoroutine == true) StartCoroutine(RefreshAllTogglesCoroutine());
            else RefreshAllToggles();
        }

    }
    public virtual void OffOtherToggles(Toggle onToggle)
    {
        foreach (var toggle in toggles)
        {
            if((toggle.gameObject != onToggle.gameObject) & (toggle.isOn == true))
            {
                toggle.isOn = false;
            }
        }
        
    }

    public virtual void RefreshAllToggles()
    {
        OnAllToggles();
        OffAllToggles();
    }

    public virtual IEnumerator RefreshAllTogglesCoroutine()
    {
        yield return new WaitForEndOfFrame();
        OnAllToggles();
        yield return new WaitForFixedUpdate();
        OffAllToggles();
    }
    public virtual void OnAllToggles()
    {
        foreach (var toggle in toggles)
        {
            toggle.isOn = true;
        }
    }

    public virtual void OffAllToggles()
    {
        foreach (var toggle in toggles)
        {
            toggle.isOn = false;
        }
    }

}


