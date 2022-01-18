using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOTS;
public class JoystickButton : MonoBehaviour
{
    // 'x+' : plus x direction
    // 'x-' : minus x direction
    // 'y+' : plus y direction
    // 'y-' : minus y direction
    // 'z+' : plus z direction
    // 'z-' : minus z direction 
    public string direction;
    public void RayCastedByClicking()
    {
        switch (direction)
        {
            case "x+":
                EditorUIManager.Instance.MoveNodeCurser_Xplus();
                break;
            case "x-":
                EditorUIManager.Instance.MoveNodeCurser_Xminus();
                break;
            case "y+":
                EditorUIManager.Instance.MoveNodeCurser_Yplus();
                break;
            case "y-":
                EditorUIManager.Instance.MoveNodeCurser_Yminus();
                break;
            case "z+":
                EditorUIManager.Instance.MoveNodeCurser_Zplus();
                break;
            case "z-":
                EditorUIManager.Instance.MoveNodeCurser_Zminus();
                break;
            default:
                break;
        }

    }


}
