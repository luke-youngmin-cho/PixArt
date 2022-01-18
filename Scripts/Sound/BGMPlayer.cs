using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] bool playOnStart;
    private void Start()
    {
        if (playOnStart == true)
            StartCoroutine(InitCoroutine());
    }
    IEnumerator InitCoroutine()
    {
        yield return new WaitUntil(() => AudioManager.instance.CMDState == CMDState.IDLE);
        AudioManager.instance.PlayBGM();
    }
}
