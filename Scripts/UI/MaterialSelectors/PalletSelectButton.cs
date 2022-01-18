using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PalletSelectButton : MonoBehaviour
{
    private string category = "PalletSelectButtonForKids";
    public int palletSelectButtonNumber;
    [HideInInspector]
    public Image image;
    private new Renderer renderer;

    private void Start()
    {
        if (palletSelectButtonNumber < 0) return;

        if (PalletManager.Instance.palletSelectButtons[palletSelectButtonNumber] != null)
            Debug.LogError($"Error: pallet select button number collision. check palletSelectButtonNumber :{palletSelectButtonNumber}");
        else
           PalletManager.Instance.palletSelectButtons[palletSelectButtonNumber] = this;
        renderer = gameObject.GetComponent<Renderer>();
        //renderer.sharedMaterial = Inventory.Instance.list_Material[palletSelectButtonNumber];
        image = gameObject.GetComponent<Image>();
    }
    public void SetMaterial(Material _material)
    {
        this.gameObject.GetComponent<Image>().color = new Color(_material.color.r, _material.color.g, _material.color.b);
        //material = new Material(_material);
        renderer.sharedMaterial = _material;
    }
    
    public void SetCursorMaterial()
    {
        if (EditorUIManager.Instance.cubeDesigneInstance != null)
        {
            EditorUIManager.Instance.cubeDesigneInstance.ChangeCursorMaterial(renderer.sharedMaterial, category);
        }
    }

    public Material GetMaterial()
    {
        if (renderer.sharedMaterial == null)
        {
            Debug.Log("no material");
            return null;
        }

        return renderer.sharedMaterial;
    }

    public void ChooseColorButtonClick()
    {
        ColorPicker.instance.Create(renderer.sharedMaterial.color, "Choose the cube's color!", SetColor, ColorFinished, true);
    }
    public void SetColor(Color currentColor)
    {
        renderer.sharedMaterial.color = currentColor;
        image.color = currentColor;
    }
    private void ColorFinished(Color finishedColor)
    {
        renderer.sharedMaterial.color = finishedColor;
        Debug.Log("You chose the color " + ColorUtility.ToHtmlStringRGBA(finishedColor));
        SetCursorMaterial();
    }
}
