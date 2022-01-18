using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
public class FontMaster : MonoBehaviour
{
    public static FontMaster instance;
    public static List<FontSlave> list_slave;
    [SerializeField] Font font_Default;
    [SerializeField] Font font_CN_Traditional;
    [SerializeField] Font font_CN_Simplified;
    [SerializeField] Font font_JP;
    [SerializeField] Font font_EL;
    [SerializeField] Font font_RU;
    [SerializeField] Font font_UK;
    [SerializeField] Font font_BE;
    [SerializeField] Font font_BG;
    Font _currentFont;
    static string _languageName = "";
    
    private void Awake()
    {
        instance = this;
        _currentFont = font_Default;
        list_slave = new List<FontSlave>();
    }
    public void Register(FontSlave slave)
    {
        list_slave.Add(slave);
    }

    public void RefreshFont(string languageName)
    {
        _languageName = languageName;
        // todo -> Improving way to get font 
        switch (languageName)
        {
            case "ChineseSimplified":
                _currentFont = font_CN_Simplified;
                break;
            case "ChineseTraditional":
                _currentFont = font_CN_Traditional;
                break;
            case "Greek":
                _currentFont = font_EL;
                break;
            case "Russian":
                _currentFont = font_RU;
                break;
            case "Ukrainian":
                _currentFont = font_UK;
                break;
            case "Belarusian":
                _currentFont = font_BE;
                break;
            case "Bulgarian":
                _currentFont = font_BG;
                break;
            /// exceptions
            case "Chinese":
                _currentFont = font_CN_Simplified;
                break;
            case "Japanese":
                _currentFont = font_JP;
                break;
            ///
            default:
                _currentFont = font_Default;
                break;
        }
        //Debug.Log($"Font is changed : {_currentFont}");
        foreach (FontSlave slave in list_slave)
        {
            slave.ChangeFont(_currentFont);
        }
    }

    public void RefreshFont()
    {
        RefreshFont(_languageName);
    }
}
