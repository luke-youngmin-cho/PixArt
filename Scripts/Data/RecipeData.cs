using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct RecipeData
{
    public string name;
    public int resoltuion;
    public int totalPageNumber;
    public string[] recipePagesURL;
    public bool ad;
    public float price;
}

[Serializable]
public struct RecipeInfo
{
    public string name;
    public int resolution;
    public int totalPageNumber;

    public Texture2D[] recipePages;
    public Texture2D recipeIcon;    
}