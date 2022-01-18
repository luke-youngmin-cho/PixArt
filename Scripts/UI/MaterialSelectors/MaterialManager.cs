using DOTS;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MaterialManager : MonoBehaviour
{
    #region singleton
    static public MaterialManager Instance;
    private void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
    }
    #endregion

    [SerializeField] List<MaterialSelectorInfo> materialSelectorInfos;

    
    private void Start()
    {
        StartCoroutine(StartRoutine());
    }


    IEnumerator StartRoutine()
    {
        foreach (var selectorInfo in materialSelectorInfos)
        {
            if (selectorInfo.normalActive == false) selectorInfo.Selector.SetActive(true);
        }

        GameObject g;
        Transform contentTransform;
        GameObject buttonSample;
        Material[] mats;

        foreach (var selectorInfo in materialSelectorInfos)
        {
            mats = Resources.LoadAll<Material>("Materials/" + selectorInfo.category);
            contentTransform = selectorInfo.scrollViewTransform.GetComponentInChildren<GridLayoutGroup>().transform;
            buttonSample = selectorInfo.scrollViewTransform.GetComponentInChildren<GridLayoutGroup>().gameObject.GetComponentInChildren<MaterialSelectButton>().gameObject;
            for (int i = 0; i < mats.Length; i++)
            {
                g = Instantiate(buttonSample, contentTransform);
                g.GetComponent<MaterialSelectButton>().InitSetting();
                g.GetComponent<Button>().AddEventListener(g, ButtonClickEvent);
                g.GetComponent<MaterialSelectButton>().SetMaterial(mats[i], selectorInfo.category, i);
            }
            Destroy(buttonSample);
            yield return null;
        }

        foreach (var selectorInfo in materialSelectorInfos)
        {
            selectorInfo.Selector.SetActive(selectorInfo.normalActive);
        }
    }


    private void ButtonClickEvent(GameObject button)
    {
        button.GetComponent<MaterialSelectButton>().SetCursorMaterial();
    }

    /////////public////////////
    ///
    public Material FindMaterialByName(string matName, string category)
    {
        return Resources.Load<Material>("Materials/" + category + "/" + matName);
    }
}

[System.Serializable]
public class MaterialSelectorInfo
{
    public string category;
    public GameObject Selector;
    public bool normalActive;
    public Transform scrollViewTransform;
}