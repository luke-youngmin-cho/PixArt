using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToSceneByNameOnMouseClick : MonoBehaviour
{
    public string targetSceneName;
    private void OnMouseDown()
    {
        SceneMover.instance.OpenSceneByName(targetSceneName);
    }
}
