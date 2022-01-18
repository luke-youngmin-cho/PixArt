using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SettingsManager : MonoBehaviour
{
    static public SettingsManager instance;
    // single ton, dont destroy on load
    private void Awake()
    {
        if (instance == null) instance = this;
        DontDestroyOnLoad(this);
        LoadSavedData();
        HideSettingsPanel();
    }
    //=========================================================================
    // internal data
    //=========================================================================
    int isPlayerPrefsDataExist;
    int sound_BGMTypeIndex;
    float sound_BGMVolume;
    float sound_SFXVolume;
    float touch_ZoomSpeed;
    float touch_JoystickSpeed;
    float touch_JoystickSensitivity;
    float touch_JoystickDraggingPrecision;

    string screen_Orientation;
    string locale_name;
    int play_Tutorial;


    //=========================================================================
    // References
    //=========================================================================
    [SerializeField] Slider sound_BGMVolume_Slider;
    [SerializeField] Slider sound_SFXVolume_Slider;
    [SerializeField] Slider touch_ZoomSpeed_Slider;
    [SerializeField] Slider touch_JoystickSpeed_Slider;
    [SerializeField] Slider touch_JoystickSensitivity_Slider;
    [SerializeField] Slider touch_JoystickDraggingPrecision_Slider;
    [SerializeField] UnityEngine.Audio.AudioMixer bgmMixer;
    [SerializeField] UnityEngine.Audio.AudioMixer sfxMixer;
    [SerializeField] Toggle play_Tutorial_Toggle;
    //=========================================================================
    // Properties to get / set PlayerPrefs
    //=========================================================================
    public int PlayerPrefsDataExist
    {
        get
        {
            return PlayerPrefs.GetInt("PlayerPrefsDataExist", isPlayerPrefsDataExist);
        }
        set
        {
            isPlayerPrefsDataExist = value;
            PlayerPrefs.SetInt("PlayerPrefsDataExist", value);
        }
    }
    public int Sound_BGMTypeIndex
    {
        get
        {
            return PlayerPrefs.GetInt("Sound_BGMTypeIndex", sound_BGMTypeIndex);
        }
        set
        {
            sound_BGMTypeIndex = value;
            e_BGMDropdownType bgmType = (e_BGMDropdownType)sound_BGMTypeIndex;
            string bgmName = bgmType.ToString();
            PlayerPrefs.SetInt("Sound_BGMTypeIndex", sound_BGMTypeIndex);
            AudioManager.instance.PlayBGM(bgmName);
        }
    }
    public float Sound_BGMVolume 
    { 
        get 
        { 
            return PlayerPrefs.GetFloat("Sound_BGMVolume", sound_BGMVolume); 
        }
        set 
        {
            sound_BGMVolume = value;
            bgmMixer.SetFloat("VolumeExposed", Mathf.Log10(sound_BGMVolume)*20);
            PlayerPrefs.SetFloat("Sound_BGMVolume", sound_BGMVolume); 
        } 
    }
    public float Sound_SFXVolume 
    { 
        get 
        { 
            return PlayerPrefs.GetFloat("sound_SFXVolume", sound_SFXVolume);
        } 
        set 
        { 
            sound_SFXVolume = value;
            sfxMixer.SetFloat("VolumeExposed", Mathf.Log10(sound_SFXVolume)*20);
            PlayerPrefs.SetFloat("sound_SFXVolume", sound_SFXVolume);
        }
    }
    public float Touch_ZoomSpeed 
    { 
        get 
        { 
            return PlayerPrefs.GetFloat("Touch_ZoomSpeed", touch_ZoomSpeed); 
        }
        set 
        { 
            touch_ZoomSpeed = value; PlayerPrefs.SetFloat("Touch_ZoomSpeed", touch_ZoomSpeed);
            if (CameraHandler.instance != null) CameraHandler.instance.zoomFactor = value;
        } 
    }
    public float Touch_JoystickSpeed
    {
        get
        {
            return PlayerPrefs.GetFloat("Touch_JoystickSpeed", touch_JoystickSpeed);
        }
        set
        {
            touch_JoystickSpeed = value;
            PlayerPrefs.SetFloat("Touch_JoystickSpeed", touch_JoystickSpeed);
            if (JoystickRayCast.instance != null) JoystickRayCast.instance.movingTerm = 1/value;
        }
    }
    public float Touch_JoystickSensitivity
    {
        get
        {
            return PlayerPrefs.GetFloat("Touch_JoysticSensitivity", touch_JoystickSensitivity);
        }
        set
        {
            touch_JoystickSensitivity = value;
            PlayerPrefs.SetFloat("Touch_JoysticSensitivity", touch_JoystickSensitivity);
            if (JoystickRayCast.instance != null) JoystickRayCast.instance.pressedDetectionDelay = value;
        }
    }
    public float Touch_JoystickDraggingPrecision
    {
        get
        {
            return PlayerPrefs.GetFloat("Touch_JoystickDraggingPrecision", touch_JoystickDraggingPrecision);
        }
        set
        {
            touch_JoystickDraggingPrecision = value;
            PlayerPrefs.SetFloat("Touch_JoystickDraggingPrecision", touch_JoystickDraggingPrecision);
            if (JoystickRayCast.instance != null) JoystickRayCast.instance.SetJoystisckPrecision(value);
        }
    }
    public string Screen_Orientation 
    { 
        get 
        { 
            return PlayerPrefs.GetString("Screen_Orientation", screen_Orientation); 
        } 
        set 
        { 
            screen_Orientation = value; 
            PlayerPrefs.SetString("Screen_Orientation", screen_Orientation); 
        }
    }

    public string Locale_Name
    {
        get
        {
            return PlayerPrefs.GetString("Locale_Name", locale_name);
        }
        set
        {
            locale_name = value;
            PlayerPrefs.SetString("Locale_Name", locale_name);
        }
    }
    public int Play_Tutorial
    {
        get
        {
            return PlayerPrefs.GetInt("Play_Tutorial", play_Tutorial);
        }
        set
        {
            play_Tutorial = value;
            PlayerPrefs.SetInt("Play_Tutorial", play_Tutorial);
            play_Tutorial_Toggle.isOn = ((play_Tutorial > 0) ? true : false);
        }
    }
    //=========================================================================
    // Elements
    //=========================================================================
    [SerializeField] private RectTransform settingsPanelRect;

    public void LoadSavedData()
    {
        if (PlayerPrefsDataExist == 0) SetupAtVeryFirstTime();
        
        sound_BGMVolume_Slider.value = Sound_BGMVolume;
        sound_SFXVolume_Slider.value = Sound_SFXVolume;
        touch_ZoomSpeed_Slider.value = Touch_ZoomSpeed;
        touch_JoystickSpeed_Slider.value = Touch_JoystickSpeed;
        touch_JoystickSensitivity_Slider.value = Touch_JoystickSensitivity;
        touch_JoystickDraggingPrecision_Slider.value = Touch_JoystickDraggingPrecision;
        play_Tutorial_Toggle.isOn = ((Play_Tutorial > 0) ? true : false);

        sound_BGMTypeIndex = Sound_BGMTypeIndex;
        sound_BGMVolume = Sound_BGMVolume;
        sound_SFXVolume = Sound_SFXVolume;
        touch_ZoomSpeed = Touch_ZoomSpeed;
        touch_JoystickSpeed = Touch_JoystickSpeed;
        touch_JoystickSensitivity = Touch_JoystickSensitivity;
        touch_JoystickDraggingPrecision = Touch_JoystickDraggingPrecision;
        screen_Orientation = Screen_Orientation;
        locale_name = Locale_Name;
        play_Tutorial = Play_Tutorial;


        /*Sound_BGMVolume = sound_BGMVolume;
        Sound_SFXVolume = sound_SFXVolume;
        Touch_ZoomSpeed = touch_ZoomSpeed;
        Touch_JoystickSpeed = touch_JoystickSpeed;
        Touch_JoystickSensitivity = touch_JoystickSensitivity;
        Screen_Orientation = screen_Orientation;*/

        /*Debug.Log(sound_BGMTypeIndex);
        Debug.Log(sound_BGMVolume);
        Debug.Log(sound_SFXVolume);
        Debug.Log(touch_ZoomSpeed);
        Debug.Log(touch_JoystickSpeed);
        Debug.Log(touch_JoystickSensitivity);*/
        Debug.Log(locale_name);

    }
    private void SetupAtVeryFirstTime()
    {
        sound_BGMTypeIndex = 0;
        Sound_BGMVolume = 0.5f;
        Sound_SFXVolume = 0.5f;
        Touch_ZoomSpeed = 0.05f;
        Touch_JoystickSpeed = 25f;
        Touch_JoystickSensitivity = 0.15f;
        Touch_JoystickDraggingPrecision = 90.0f;
        PlayerPrefsDataExist = 1;
        Play_Tutorial = 1;
        locale_name = GetLocaleNameWithDeviceLanguage();

    } 

    private string GetLocaleNameWithDeviceLanguage()
    {
        string systemLanguage = Application.systemLanguage.ToString();
        systemLanguage = LocaleSettings.instance.GetConvertedLanguageName(systemLanguage);
        return systemLanguage;
    }

    public void ShowSettingsPanel()
    {
        settingsPanelRect.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    public void HideSettingsPanel()
    {
        SaveSettings();
        settingsPanelRect.position = new Vector2(Screen.width *5, Screen.height *5);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        //Debug.Log("Player prefs saved");
    }
}
