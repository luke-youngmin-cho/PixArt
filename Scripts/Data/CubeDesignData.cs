using DOTS;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct BricksData
{
    public List<int> list_BrickInfo_id;
    public List<bool> list_BrickInfo_isExist;
    public List<string> list_BrickInfo_meshName;
    public List<string> list_BrickInfo_matCategory;
    public List<string> list_BrickInfo_matName;
    public List<float3> list_BrickInfo_rotationValue;
    public List<float> list_BrickInfo_scaleFactor;
}

[System.Serializable]
public class CubeDesignData
{
    public bool isSuccessfullyConstructed;
    public string cubeDesingName;
    public int resolution;
    public BricksData bricksData;

    public CubeDesignData(bool wantToGetOpenedCubeDesignDataAtEdtior)
    {   
        if (wantToGetOpenedCubeDesignDataAtEdtior == true)
        {
            isSuccessfullyConstructed = false;
        }
    }
}
public struct SavedDesignsInfo
{
    public string name;
    public string description;
    public string IconURL;

    [NonSerialized]
    public Texture2D Icon;
}