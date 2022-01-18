using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class Test_LoadCapturedImage : MonoBehaviour
{
    [SerializeField] RawImage img;
    public void OnButtonClick()
    {
        byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/" + "DesignIcon_TransparentTest2" + ".png");
        Texture2D texture = new Texture2D((int)img.rectTransform.rect.width, (int)img.rectTransform.rect.height);
        texture.LoadImage(bytes);
        img.GetComponent<RawImage>().texture  = texture;
    }
}
