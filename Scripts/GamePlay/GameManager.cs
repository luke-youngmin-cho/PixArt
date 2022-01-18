using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(instance);
            instance = this;
        }
        DontDestroyOnLoad(instance);
    }
    private bool isPaused = false; // to check application paused
    private float pauseCount = 0f;
    [HideInInspector]public e_Mode _mode;
    [HideInInspector]public e_ScreenDirectionType screenDireection;
    private ScreenOrientation screenOrientation;

    /// <summary>
    /// Detecting pause/ quit game
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        isPaused = pause;
    }

    public void SetMode(e_Mode mode)
    {
        _mode = mode;
    }
    public void SetMode(string modeName)
    {
        if (modeName == "Pro") _mode = e_Mode.Pro;
        else _mode = e_Mode.Kids;
    }

    public void SetScreenDirection(e_ScreenDirectionType screenDirection)
    {
        switch (screenDirection)
        {
            case e_ScreenDirectionType.Rotational:
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
            case e_ScreenDirectionType.PortraitFix:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case e_ScreenDirectionType.LandscapeFix:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
            default:
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
        }
    }
    public void SetScreenDirection(int screenDirectionValue)
    {
        switch (screenDirectionValue)
        {
            case 0:
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
            case 1:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case 2:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
            default:
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
        }
    }


}
[System.Serializable]
public enum e_Mode
{
    Kids,
    Pro
}
public enum e_ScreenDirectionType
{
    Rotational,
    PortraitFix,
    LandscapeFix
}
