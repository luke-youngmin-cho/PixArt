using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipesPanel : MonoBehaviour
{
    static public RecipesPanel instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void OpenThis()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(DOTS.DataManager.instance.GetRecipesInfo());
    }
    public void CloseThis()
    {
        this.gameObject.SetActive(false);
        DOTS.DataManager.instance.RemoveRecipesUI();
    }
}
