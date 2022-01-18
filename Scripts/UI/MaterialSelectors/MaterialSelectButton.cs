using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MaterialSelectButton : MonoBehaviour
{
    [HideInInspector] public Material _mat;
    [HideInInspector] public int _id;
    [HideInInspector] public string _category;
    [HideInInspector] public Renderer rend;
    [HideInInspector] public Image img;
    
    virtual public void InitSetting()
    {
        rend = gameObject.GetComponent<Renderer>();
        img = gameObject.GetComponent<Image>();
    }
    virtual public void SetMaterial(Material mat, string category, int id)
    {   
        _id = id;
        _mat = mat;
        _category = category;
        img.color = _mat.color;
    }
    virtual public void SetCursorMaterial()
    {
        if (EditorUIManager.Instance.cubeDesigneInstance != null)
        {
            EditorUIManager.Instance.cubeDesigneInstance.ChangeCursorMaterial(_mat, _category);
        }
    }
}
