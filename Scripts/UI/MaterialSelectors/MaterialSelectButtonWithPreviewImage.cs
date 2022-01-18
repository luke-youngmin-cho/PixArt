using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelectButtonWithPreviewImage : MaterialSelectButton
{
    override public void SetMaterial(Material mat, string category, int id)
    {
        _id = id;
        _mat = mat;
        _category = category;
        
        img.sprite = Resources.Load<Sprite>("Materials/"+ category +"/" + category + "Preview/" + mat.name);
    }
}
