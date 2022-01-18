using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResolutionInputBase : MonoBehaviour
{
    [SerializeField] public GameObject panel;
    [SerializeField] public Text t;
    [SerializeField] public List<Text> displayText;
    [SerializeField] int limit;
    [SerializeField] public GameObject limitPopUpWindow;
    [HideInInspector]
    public string mem_t;
    [HideInInspector]
    public int resolution;
    private void Awake()
    {
        resolution = 0;
        limitPopUpWindow.GetComponentInChildren<Text>().text = "Limit : " + limit.ToString();
    }
    private void OnEnable()
    {
        mem_t = t.text;
    }
    virtual public void OnNumberButtonClicked(int num)
    {
        bool isNum;
        int tmpNum;
        string memStr = t.text;
        isNum = int.TryParse(memStr, out tmpNum);
        if (isNum == false)
        {
            memStr = "";
        }
        memStr += num.ToString();
        isNum = int.TryParse(memStr, out tmpNum);
        if ((isNum == false) | (tmpNum > limit))
        {
            limitPopUpWindow.SetActive(true);
            return;
        }

        resolution = int.Parse(memStr);
        t.text = memStr;
        foreach (Text item in displayText)
        {
            item.text = memStr;
        }
    }
    virtual public void OnCButtonClicked()
    {
        t.text = "";
    }
    virtual public void OnOkClicked()
    {
        mem_t = t.text;
        panel.SetActive(false);
    }
    virtual public void OnCancelClicked()
    {
        // roll back text
        t.text = mem_t;
        foreach (Text item in displayText)
        {
            item.text = mem_t;
        }
        panel.SetActive(false);
    }
}
