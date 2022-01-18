using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpToolForAskingTutorial : PopUpTool
{
    public override void SetPosition()
    {
        if (SettingsManager.instance.Play_Tutorial > 0)
            base.SetPosition();
        else
            this.gameObject.SetActive(false);
    }
}
