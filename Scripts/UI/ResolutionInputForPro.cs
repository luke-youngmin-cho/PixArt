using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionInputForPro : ResolutionInputBase
{
    //[SerializeField] Toggle ResolutionInputToggle;
    override public void OnOkClicked()
    {
        mem_t = t.text;
        //ResolutionInputToggle.isOn = false;
        EditorUIManager.Instance.SelectResolution(resolution);
        panel.SetActive(false);
    }
    override public void OnCancelClicked()
    {
        // roll back text
        t.text = mem_t;
        foreach (Text item in displayText)
        {
            item.text = mem_t;
        }
        //ResolutionInputToggle.isOn = false;
        panel.SetActive(false);
    }
}
