using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusAnimationForImage : MonoBehaviour
{
    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] float animationSpeed;
    [SerializeField] float scalingUnit;
    RectTransform rect;
    bool updownFlag = true;
    Coroutine coroutine;
    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (coroutine != null) return;
        if(updownFlag == true)
        {
            coroutine = StartCoroutine(scaleDown());
        }
        else
        {
            coroutine = StartCoroutine(scaleUp());
        }
    }
    IEnumerator scaleDown()
    {
        yield return new WaitForSeconds(1/animationSpeed * Time.deltaTime );
        rect.localScale -= new Vector3(scalingUnit, scalingUnit, scalingUnit);
        if (rect.localScale.x <= minScale)
        {
            updownFlag = false;
        }
        StopCoroutine(coroutine);
        coroutine = null;
    }
    IEnumerator scaleUp()
    {
        yield return new WaitForSeconds(1 / animationSpeed * Time.deltaTime);
        rect.localScale += new Vector3(scalingUnit, scalingUnit, scalingUnit);
        if (rect.localScale.x >= maxScale)
        {
            updownFlag = true;
        }
        StopCoroutine(coroutine);
        coroutine = null;
    }

}
