using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocaleSettings : MonoBehaviour
{
    static public LocaleSettings instance;
    static public CMDState initState = CMDState.BUSY;
    Coroutine initCoroutine;
    int defaultLanguageIndex = -1;
    List<char> charWantToRemove = new List<char>() {' ', '(', ')'};
    private void Awake()
    {
        if (instance == null) instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        initCoroutine = StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return LocalizationSettings.InitializationOperation;
        //Debug.Log("Localization initialized!!!!!!!!!");
        yield return new WaitUntil(() => (SettingsManager.instance != null));
        //Debug.Log("Locale Settings Init started");

        string tmpLanguageName = "";
        DrawUI_LanguageButtons();
        if (SettingsManager.instance.PlayerPrefsDataExist > 0)
        {
            tmpLanguageName = SettingsManager.instance.Locale_Name;
        }
        else
        {
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                string tmpLocaleName = LocalizationSettings.AvailableLocales.Locales[i].name;
                tmpLocaleName = GetConvertedLanguageName(tmpLocaleName);
                string tmpSystemLanguageName = Application.systemLanguage.ToString();
                tmpSystemLanguageName = GetConvertedLanguageName(tmpSystemLanguageName);
                if (tmpSystemLanguageName == tmpLocaleName)
                {
                    tmpLanguageName = tmpLocaleName;

                }
            }
        }
        defaultLanguageIndex = GetLanguageIndex("English");
        SetLocaleIndex(tmpLanguageName);
        initState = CMDState.IDLE;
        StopCoroutine(initCoroutine);
        initCoroutine = null;
    }
    public bool IsInitFinished()
    {
        bool isFinished = false;
        if(initState == CMDState.IDLE)
        {
            isFinished = true;
        }
        return isFinished;
    }
    public void SetLocaleIndex(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        string languageName = GetConvertedLanguageName(LocalizationSettings.SelectedLocale.name);
        SettingsManager.instance.Locale_Name = languageName;
        SettingsManager.instance.SaveSettings();
        FontMaster.instance.RefreshFont(languageName);
    }

    public void SetLocaleIndex(string name)
    {
        int index = GetLanguageIndex(name);
        SetLocaleIndex(index);
    }

    [SerializeField] Transform content;
    [SerializeField] GameObject languageButton;
    List<GameObject> tmpList_LanguageButton;
    private void DrawUI_LanguageButtons()
    {
        tmpList_LanguageButton = new List<GameObject>();
        int tmpCount = LocalizationSettings.AvailableLocales.Locales.Count;
        GameObject tmpGameObejct;
        float tmpButtonHeight = languageButton.GetComponent<RectTransform>().rect.height;
        RectTransform tmpRectTransfrom = content.GetComponent<RectTransform>();
        string tmpLanguageName = "";
        for (int i = 0; i < tmpCount; i++)
        {
            tmpLanguageName = LocalizationSettings.AvailableLocales.Locales[i].name;
            tmpGameObejct = Instantiate(languageButton, content);
            tmpGameObejct.GetComponent<Button>().AddEventListener(tmpLanguageName,
                                                      LanguageButtonEvent);
            tmpGameObejct.transform.GetChild(0).GetComponent<Text>().text = tmpLanguageName;
            tmpGameObejct.SetActive(true);
            //Debug.Log($"Locale settings : locale name : {g.transform.GetChild(0).GetComponent<Text>().text}");
            tmpList_LanguageButton.Add(tmpGameObejct);
        }
        // resize content
        tmpRectTransfrom.sizeDelta = new Vector2(tmpRectTransfrom.sizeDelta.x,
                                              tmpButtonHeight * (tmpCount + 1) );
        Destroy(languageButton);
    }

    private void LanguageButtonEvent(string name)
    {
        string tmpName = name;
        SettingsManager.instance.Locale_Name = GetConvertedLanguageName(tmpName);
        SetLocaleIndex(name);
    }

    public int GetLanguageIndex(string languageName)
    {
        //Debug.Log($" You tried to get a index of language {languageName}");
        int index = -1;
        string tmpLanguageName = GetConvertedLanguageName(languageName);
        string tmpAvailableLocaleName = "";
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            tmpAvailableLocaleName = LocalizationSettings.AvailableLocales.Locales[i].ToString();
            tmpAvailableLocaleName = GetConvertedLanguageName(tmpAvailableLocaleName);
            //matched = LocalizationSettings.AvailableLocales.Locales[i].ToString().Contains(languageName);
            //Debug.Log($"language list {i} : {tmpAvailableLocaleName} compare with {tmpLanguageName}");
            if(tmpAvailableLocaleName.Contains(tmpLanguageName))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public string GetConvertedLanguageName(string name)
    {
        string languageName = name;
        int bracketOpenIndex = languageName.LastIndexOf("(");
        int bracketCloseIndex = name.LastIndexOf(")");
        if(bracketOpenIndex > 0)
            languageName = languageName.Remove(bracketOpenIndex);
        languageName = StringExtensions.Filter(languageName, charWantToRemove);
        return languageName;
    }
}
