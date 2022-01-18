using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PalletSelectButtonForPro : MonoBehaviour
{
    private string category = "PalletSelectButtonForPro";
    public int palletSelectButtonNumber;
    [HideInInspector]
    public Image image;
    private new Renderer renderer;

    //shaders
    private Shader shaderDefault;
    private Shader shaderTransparent;

    private void Start()
    {
        if (palletSelectButtonNumber < 0) return;

        if (PalletManagerForPro.Instance.palletSelectButtons[palletSelectButtonNumber] != null)
            Debug.LogError($"Error: pallet select button number collision. check palletSelectButtonNumber :{palletSelectButtonNumber}");
        else
            PalletManagerForPro.Instance.palletSelectButtons[palletSelectButtonNumber] = this;
        renderer = gameObject.GetComponent<Renderer>();
        //renderer.sharedMaterial = Inventory.Instance.list_Material[palletSelectButtonNumber];
        shaderDefault     = Shader.Find("Diffuse");
        shaderTransparent = Shader.Find("Transparent/Diffuse");
        

        image = gameObject.GetComponent<Image>();
    }
    public void SetMaterial(Material _material)
    {
        this.gameObject.GetComponent<Image>().color = new Color(_material.color.r, _material.color.g, _material.color.b, _material.color.a);
        //material = new Material(_material);

        renderer.sharedMaterial = _material;
    }
    
    public void SetCursorMaterial()
    {
        if (EditorUIManager.Instance.cubeDesigneInstance != null)
        {
            if(renderer.sharedMaterial.color.a == 1.0f)
            {
                //renderer.sharedMaterial.shader = Shader.Find("Diffuse");
                ToOpaqueMode(renderer.sharedMaterial);
            }
            else
            {
                //renderer.sharedMaterial.shader = Shader.Find("Transparent/Diffuse");
                ToFadeMode(renderer.sharedMaterial);
            }
            Debug.Log($"shader is changed as {renderer.sharedMaterial.shader.name}");

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

    private void ToOpaqueMode(Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }

    private void ToFadeMode(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
