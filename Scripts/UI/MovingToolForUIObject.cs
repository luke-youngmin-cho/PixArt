using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingToolForUIObject : MonoBehaviour
{
    RectTransform r;
    Vector2 origin;
    private void Awake()
    {
        r = GetComponent<RectTransform>();
        origin = new Vector2(r.position.x, r.position.y);
    }
    public void MoveToCentorOfScreen()
    {
        Debug.Log(" ask user to play to play pro panel moves center");
        r.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }
    public void MoveBackToOrigin()
    {
        Debug.Log(" ask user to play to play pro panel moves back to origin");
        r.position = origin;
    }

}
