using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Test_CaptureAndSaveImage : MonoBehaviour
{
    [SerializeField] InputField pathToSave;
    //[SerializeField] InputField fileName;
    [SerializeField] MeshRenderer target;
    private Texture2D _tex;
    private Coroutine coroutine;
    
    public void CaptureAndSaveSquare()
    {
            coroutine = StartCoroutine(CaptureScreenshotAsTextureAfterEndOfFrame(_tex));
        
    }

    IEnumerator CaptureScreenshotAsTextureAfterEndOfFrame(Texture2D tex)
    { 
        if (Directory.Exists(Application.persistentDataPath + "/TestImages/" + pathToSave.text) == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/TestImages/" + pathToSave.text);
        }

        string name = target.material.name.Replace(" (Instance)","");
        string path =  Application.persistentDataPath +"/TestImages/" + pathToSave.text + "/" + name + ".png";
        yield return new WaitForEndOfFrame();
        tex = ScreenCapture.CaptureScreenshotAsTexture();
        tex = TextureTool.ResampleAndCrop(tex, Screen.width, Screen.width);
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        yield return null;
    }


}
