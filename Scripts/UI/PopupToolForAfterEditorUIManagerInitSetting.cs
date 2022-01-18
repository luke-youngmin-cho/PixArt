using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupToolForAfterEditorUIManagerInitSetting : PopUpTool
{
    private void OnEnable()
    {
        SetPosition();
        if (autoClose == true)
        {
            StartCoroutine(CloseAfterDatamanagerInitSetting(autoCloseDelayTime));
        }
        if (autoDestroy == true)
        {
            StartCoroutine(DestroyAfterDatamanagerInitSetting(autoDestroyTime));
        }
    }

    IEnumerator CloseAfterDatamanagerInitSetting(float seconds)
    {
        yield return new WaitUntil(() => EditorUIManager.Instance.initState == CMDState.IDLE);
        yield return new WaitForSeconds(seconds);
        this.gameObject.SetActive(false);
        yield return null;
    }
    IEnumerator DestroyAfterDatamanagerInitSetting(float seconds)
    {
        yield return new WaitUntil(() => EditorUIManager.Instance.initState == CMDState.IDLE);
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
        yield return null;
    }
}
