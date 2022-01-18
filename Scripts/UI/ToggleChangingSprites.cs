using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleChangingSprites : MonoBehaviour
{
    public Toggle toggle;
    public List<Image> list_toggleOnImage;
    public List<Image> list_toggleOffImage;
    public List<Text> list_textWhatYouWantToChangeItsColor;
    public Color textOnColor;
    public Color textOffColor;
    public List<GameObject> list_whatYouWantToOnOff;

    private void Start()
    {
        if (toggle.isOn) On();
        else Off();
    }
    public void OnOff()
    {
        if(toggle.isOn)
        {
            On();
        }
        else
        {
            Off();
        }
    }
    public void On()
    {
        ActiveOnImages();
        TextOnColorChange();
        ObjectsOn();
    }
    public void Off()
    {
        ActiveOffImages();
        TextOffColorChange();
        ObjectsOff();
    }
    private void ActiveOnImages()
    {
        foreach (var image in list_toggleOffImage)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var image in list_toggleOnImage)
        {
            image.gameObject.SetActive(true);
        }
    }
    private void ActiveOffImages()
    {
        foreach (var image in list_toggleOnImage)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var image in list_toggleOffImage)
        {
            image.gameObject.SetActive(true);
        }
    }

    private void TextOnColorChange()
    {
        foreach (var text in list_textWhatYouWantToChangeItsColor)
        {
            text.color = textOnColor;
        }
    }
    private void TextOffColorChange()
    {
        foreach (var text in list_textWhatYouWantToChangeItsColor)
        {
            text.color = textOffColor;
        }
    }

    private void ObjectsOn()
    {
        foreach (var obj in list_whatYouWantToOnOff)
        {
            obj.SetActive(true);
        }
    }
    private void ObjectsOff()
    {
        foreach (var obj in list_whatYouWantToOnOff)
        {
            obj.SetActive(false);
        }
    }


}
