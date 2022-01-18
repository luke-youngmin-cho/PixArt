using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayProButton : MonoBehaviour
{
    public GameManager gameManager;
    public bl_SceneLoader sceneLoader;
    public AudioManager audioManager;
    public GameObject panel_AskUserToPlayAdToPlayPro;
    public void OnClick()
    {   
        if (Purchaser.instance.IsAdRemoved())
        {
            gameManager.SetMode("Pro");
            sceneLoader.LoadLevel("CubeEditorForPro");
            audioManager.StopAll();
        }
        else
        {
            panel_AskUserToPlayAdToPlayPro.GetComponent<MovingToolForUIObject>().MoveToCentorOfScreen();
        }
    }
}
