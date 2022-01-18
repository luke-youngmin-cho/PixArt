using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectedBrickWindow : MonoBehaviour
{
    [SerializeField] RawImage windowImage;
    [SerializeField] Image defaultImage;

    private void Start()
    {
        ChangeImage(defaultImage);
    }
    public void ChangeImage(Image image)
    {
        windowImage.texture = image.mainTexture;
    }

}
