using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionInputForKids : ResolutionInputBase
{
    override public void OnOkClicked()
    {
        mem_t = t.text;
        InitPanel.instance.OnSelectResolutionButtonClicked(resolution);
        panel.SetActive(false);
    }
}
