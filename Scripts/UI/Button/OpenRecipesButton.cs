using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenRecipesButton : MonoBehaviour
{
    public GameObject recipesPanelObject;
    public GameObject recipePanelObject;
    private RecipesPanel recipesPanel;
    private void Awake()
    {
        recipesPanel = recipesPanelObject.GetComponent<RecipesPanel>();
    }
    public void OnClickEvent()
    {
        if(recipesPanelObject.activeSelf == true)
        {
            recipesPanel.CloseThis();
        }
        else
        {
            if (recipePanelObject.activeSelf == false)
                recipesPanel.OpenThis();
            else
                recipePanelObject.SetActive(false);
        }
    }
}
