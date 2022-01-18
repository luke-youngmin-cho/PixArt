using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMover : MonoBehaviour
{
    static public SceneMover instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Camera maincam;
    public CanvasFadeBlackUI fadeBlackUI;
    public Text navigatorText;
    

    public void OpenCubeEditorScene()
    {
        StartCoroutine(fadeBlackUI.FadeOutAndLoadScene("CubeEditor"));
    }
    public void OpenCubeEditorForKidsScene()
    {
        StartCoroutine(fadeBlackUI.FadeOutAndLoadScene("CubeEditorForKids"));
    }
    public void OpenCubeEditorForProScene()
    {
        StartCoroutine(fadeBlackUI.FadeOutAndLoadScene("CubeEditorForPro"));
    }
    public void OpenMainScene()
    {
        StartCoroutine(fadeBlackUI.FadeOutAndLoadScene("Main"));
    }
    public void OpenSettingsScene()
    {
        StartCoroutine(fadeBlackUI.FadeOutAndLoadScene("Settings"));
    }

    public void OpenSceneByName(string sceneName)
    {
        StartCoroutine(fadeBlackUI.FadeOutAndLoadScene(sceneName));
    }
}
