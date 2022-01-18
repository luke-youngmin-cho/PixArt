using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStartWorks : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(InitCorouinte());
    }

    IEnumerator InitCorouinte()
    {
        yield return new WaitUntil(() => LocaleSettings.initState == CMDState.IDLE);
        yield return new WaitUntil(() => FontMaster.instance != null);
        FontMaster.instance.RefreshFont();
    }
}
