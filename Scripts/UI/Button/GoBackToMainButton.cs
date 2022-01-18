using UnityEngine;

public class GoBackToMainButton : MonoBehaviour
{
    [SerializeField] private GameObject doubleCheckingPanel;
    [SerializeField] private bl_SceneLoader sceneLoader;
    public void OnButtonClicked()
    {
        DOTS.CubeDesign cubeDesign = EditorUIManager.Instance.GetCurrentCubeDesign();

        if ((cubeDesign !=null) & (cubeDesign._isSaved == true)){
            sceneLoader.LoadLevel("Main");
        }
        else
        {
            doubleCheckingPanel.SetActive(true);
        }
    }

}
