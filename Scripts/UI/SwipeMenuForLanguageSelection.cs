using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// you should add dummy item at the first and the last.
public class SwipeMenuForLanguageSelection : MonoBehaviour
{
    /*float itemLength;
    float spaceLength;
    float positionWindow;
    int selectedIndex;
    CMDState initState = CMDState.BUSY;
    Coroutine initCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        itemLength = transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        spaceLength = GetComponent<HorizontalLayoutGroup>().spacing;
        positionWindow = (itemLength + spaceLength) / 3;
        initCoroutine = StartCoroutine(SetSavedIndex());
    }
    IEnumerator SetSavedIndex()
    {
        yield return new WaitUntil(() => SettingsManager.instance != null);
        yield return new WaitUntil(() => LocaleSettings.instance != null);
        yield return new WaitUntil(() => LocaleSettings.instance.IsInitFinished() == true);
        if(SettingsManager.instance.PlayPrefDataExist > 0)
        {
            selectedIndex = SettingsManager.instance.Locale_Index;
        }
        else
        {
            selectedIndex = LocaleSettings.instance.GetLocaleIndex();
        }
        
        transform.localPosition = new Vector3((-1) * (itemLength + spaceLength) * selectedIndex,
                                              transform.localPosition.y,
                                              transform.localPosition.z);
        initState = CMDState.IDLE;
        StopCoroutine(initCoroutine);
        initCoroutine = null;
    }
    public bool IsInitFinished()
    {
        bool isFinished = false;
        if (initState == CMDState.IDLE)
            isFinished = true;
        return isFinished;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsInitFinished() == false) return;
        if (!Input.GetMouseButton(0))
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                if (transform.localPosition.x < (-1) * ((itemLength + spaceLength) * i - positionWindow) &&
                    transform.localPosition.x > (-1) * ((itemLength + spaceLength) * i + positionWindow))
                {
                    transform.localPosition = new Vector3((-1) * (itemLength + spaceLength) * i,
                                                 transform.localPosition.y,
                                                 transform.localPosition.z);
                    ChangeLocaleIndex(i);
                }
            }
        }
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if (i + 1 > transform.childCount - 1) return;

            if (transform.localPosition.x < (-1) * ((itemLength + spaceLength) * i - positionWindow) &&
                transform.localPosition.x > (-1) * ((itemLength + spaceLength) * i + positionWindow))
            {
                transform.GetChild(i + 1).localScale = Vector2.Lerp(transform.GetChild(i + 1).localScale, new Vector2(1.2f, 1.2f), 0.1f);
            }
            else
            {
                transform.GetChild(i + 1).localScale = Vector2.Lerp(transform.GetChild(i + 1).localScale, new Vector2(0.8f, 0.8f), 0.1f);
            }
        }
    }
    private void ChangeLocaleIndex(int index)
    {
        if ((index != selectedIndex) &
                       (index < transform.childCount - 2) &
                       (LocaleSettings.instance != null))
        {
            Debug.Log("Language Changed!");
            LocaleSettings.instance.SetLocaleIndex(index);
            selectedIndex = index;
        }
    }
    public void Move(int index)
    {
        transform.localPosition = new Vector3((-1) * (itemLength + spaceLength) * index,
                                              transform.localPosition.y,
                                              transform.localPosition.z);
        ChangeLocaleIndex(index);
    }
    public void MoveLeft()
    {
        int tmpIndex = selectedIndex;
        if (tmpIndex - 1 >= 0)
        {
            tmpIndex--;
            transform.localPosition = new Vector3((-1) * (itemLength + spaceLength) * tmpIndex,
                                              transform.localPosition.y,
                                              transform.localPosition.z);
            ChangeLocaleIndex(tmpIndex);
        }
    }
    public void MoveRight()
    {
        int tmpIndex = selectedIndex;
        if (tmpIndex + 1 < transform.childCount - 2)
        {
            tmpIndex++;
            transform.localPosition = new Vector3((-1) * (itemLength + spaceLength) * tmpIndex,
                                              transform.localPosition.y,
                                              transform.localPosition.z);
            ChangeLocaleIndex(tmpIndex);
        }
    }*/

}

