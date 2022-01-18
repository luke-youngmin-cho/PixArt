using DOTS;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] GameObject savePanel;
    public void OnButtonClicked()
    {
        if (EditorUIManager.Instance.GetCurrentCubeDesign().IsSaved() == true) return;

        if (DataManager.instance.IsLatestSavedCubeDesignExist() == true)
            DataManager.instance.SaveCubeDesignData();
        else
            savePanel.SetActive(true);
    }
}
