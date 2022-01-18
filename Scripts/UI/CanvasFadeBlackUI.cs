using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CanvasFadeBlackUI : MonoBehaviour
{
    public Image fadingPanel;
    public AnimationCurve fadeSpeedCurve;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float t = 1f;
        while (t > 0)
        {
            t -= Time.deltaTime;
            float a = fadeSpeedCurve.Evaluate(t);
            fadingPanel.color = new Color(0, 0, 0, a);
            yield return 0;
        }
        yield return null;
    }

    public IEnumerator FadeOutAndLoadScene(int sceneIdx)
    {
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime;
            Debug.Log(t);
            float a = fadeSpeedCurve.Evaluate(t);
            fadingPanel.color = new Color(0, 0, 0, a);
            yield return 0;
        }
        //SceneManager.LoadScene(sceneIdx);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIdx);
        
        yield return null;
    }
    public IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime;
            Debug.Log(t);
            float a = fadeSpeedCurve.Evaluate(t);
            fadingPanel.color = new Color(0, 0, 0, a);
            yield return 0;
        }
        //SceneManager.LoadScene(sceneIdx);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        yield return null;
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime;
            float a = fadeSpeedCurve.Evaluate(t);
            fadingPanel.color = new Color(0, 0, 0, a);
            yield return 0;
        }

        yield return null;
    }
}