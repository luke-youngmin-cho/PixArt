using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// how to use:
/// instanciate this when window is enabled.
/// </summary>
public class EventDetectorForWindow : MonoBehaviour
{
    public delegate void EventCall();

    private void Awake()
    {
        CameraHandler.instance.EnableHandling();
    }

    public void EventTriggered(GameObject obj)
    {
        obj.SetActive(false);
        CameraHandler.instance.EnableHandling();
        Destroy(this.gameObject);
    }
    
    public void EventTriggered(EventCall func)
    {
        func();
        CameraHandler.instance.EnableHandling();
        Destroy(this.gameObject);
    }
    public void EventTriggered(GameObject obj, EventCall func)
    {
        func();
        obj.SetActive(false);
        CameraHandler.instance.EnableHandling();
        Destroy(this.gameObject);
    }
}
