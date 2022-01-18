using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleManager_WaitForOnInitFinish : ToggleManager
{
    public override IEnumerator RefreshAllTogglesCoroutine()
    {
        yield return new WaitUntil(() => DOTS.DataManager.instance.initState == CMDState.IDLE);
        OnAllToggles();
        yield return new WaitForFixedUpdate();
        OffAllToggles();
    }

}
