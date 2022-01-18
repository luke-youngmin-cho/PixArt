using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeZoomPanel : MonoBehaviour
{
    public Transform recipePage;
    private float zoom;
    private float initZoom;
    private bool isEnableToHandling = false;

    private void Awake()
    {
        initZoom = recipePage.transform.localScale.x;
        zoom = initZoom;
    }
    /*private void Update()
    {
        if(isEnableToHandling == true)
        {
#if (UNITY_EDITOR)
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Debug.Log("Recipe Zoom + !");
                recipePage.transform.localScale += new Vector3(zoomFactor * 10f , zoomFactor * 10f, 1f);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                recipePage.transform.localScale -= new Vector3(zoomFactor * 10f, zoomFactor * 10f, 1f);
            }

#elif (UNITY_IOS || UNITY_ANDROID)

            if (Input.touchCount != 2) return;

            float disX = 0;
            float disY = 0;

            if (Input.GetTouch(0).position.x > Input.GetTouch(1).position.x)
            {
                disX = Input.GetTouch(0).deltaPosition.x - Input.GetTouch(1).deltaPosition.x;
            }
            else
            {
                disX = -Input.GetTouch(0).deltaPosition.x + Input.GetTouch(1).deltaPosition.x;
            }

            if (Input.GetTouch(0).position.y < Input.GetTouch(1).position.y)
            {
                disY = Input.GetTouch(0).deltaPosition.y - Input.GetTouch(1).deltaPosition.y;
            }
            else
            {
                disY = -Input.GetTouch(0).deltaPosition.y + Input.GetTouch(1).deltaPosition.y;
            }
            var zoom = disX + disY;

            recipePage.transform.localScale += new Vector3(zoom * zoomFactor, zoom * zoomFactor, 1f);
#endif
        }
    }*/

    public void RecipeImageSizeUpdate()
    {
        if(zoom >= initZoom * 3f)
        {
            zoom = initZoom;
        }
        else
        {
            zoom += initZoom * 0.5f;
        }
        recipePage.transform.localScale = new Vector3(zoom, zoom, 1f); 
    }

    public void EnableRecipePageHandling()
    {
        isEnableToHandling = true;
    }
    public void DisableRecipePageHandling()
    {
        isEnableToHandling = false;
    }
}
