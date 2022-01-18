using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
public class LightBulbHandler : MonoBehaviour
{   
    // bulb for visualization
    [SerializeField] Transform bulb;

    // lights
    [SerializeField] Light camLight;
    [SerializeField] Light customLight;

    // Toggle lights
    public bool isCustomMode { get { return m_isCustomMode; } set { m_isCustomMode = SetCustomlight(value); } }
    private bool m_isCustomMode = false;

    // rotating custom light
    Coroutine coroutine;
    
    public bool OnRotateXPlus { set { m_OnRotateXPlus = value; } }
    public bool OnRotateXMinus { set { m_OnRotateXMinus = value; } }
    public bool OnRotateYPlus {  set { m_OnRotateYPlus = value; } }
    public bool OnRotateYMinus { set { m_OnRotateYMinus = value; } }
    public bool OnRotateZPlus { set { m_OnRotateZPlus = value; } }
    public bool OnRotateZMinus { set { m_OnRotateZMinus = value; } }

    private bool m_OnRotateXPlus = false;
    private bool m_OnRotateXMinus = false;
    private bool m_OnRotateYPlus = false;
    private bool m_OnRotateYMinus = false;
    private bool m_OnRotateZPlus = false;
    private bool m_OnRotateZMinus = false;

    private void Awake()
    {
        bulb.transform.rotation = camLight.gameObject.transform.rotation;
    }
    private void Update()
    {
        if (coroutine != null) return;

        if (m_OnRotateXPlus) RotateBulbXPlus();
        else if (m_OnRotateXMinus) RotateBulbXMinus();
        else if (m_OnRotateYPlus) RotateBulbYPlus();
        else if (m_OnRotateYMinus) RotateBulbYMinus();
        else if (m_OnRotateZPlus) RotateBulbZPlus();
        else if (m_OnRotateZMinus) RotateBulbZMinus();

        UpdateBulbRotation();
    }
    public void RotateBulbXPlus()
    {
        coroutine = StartCoroutine(RotateBulbXPlusCoroutine());
    }

    public void RotateBulbXMinus()
    {
        coroutine = StartCoroutine(RotateBulbXMinusCoroutine());
    }
    public void RotateBulbYPlus()
    {
        coroutine = StartCoroutine(RotateBulbYPlusCoroutine());
    }
    public void RotateBulbYMinus()
    {
        coroutine = StartCoroutine(RotateBulbYMinusCoroutine());
    }
    public void RotateBulbZPlus()
    {
        coroutine = StartCoroutine(RotateBulbZPlusCoroutine());
    }
    public void RotateBulbZMinus()
    {
        coroutine = StartCoroutine(RotateBulbZMinusCoroutine());
    }
    private void UpdateBulbRotation()
    {
        if(m_isCustomMode == true)
        {
            bulb.transform.rotation = customLight.gameObject.transform.rotation;
        }
        else
        {
            bulb.transform.rotation = camLight.gameObject.transform.rotation;
        }
    }
    IEnumerator RotateBulbXPlusCoroutine()
    {
        if(m_isCustomMode == true)
        {
            customLight.gameObject.transform.Rotate(new Vector3(1.0f, 0f, 0f), Space.World);
        }
        else
        {
            camLight.gameObject.transform.Rotate(new Vector3(1.0f, 0f, 0f), Space.World);
        }
        
        yield return new WaitForSeconds(0.02f);
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    IEnumerator RotateBulbXMinusCoroutine()
    {
        if (m_isCustomMode == true)
        {
            customLight.gameObject.transform.Rotate(new Vector3(-1.0f, 0f, 0f), Space.World);
        }
        else
        {
            camLight.gameObject.transform.Rotate(new Vector3(-1.0f, 0f, 0f), Space.World);
        }
        yield return new WaitForSeconds(0.01f);
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    IEnumerator RotateBulbYPlusCoroutine()
    {
        if (m_isCustomMode == true)
        {
            customLight.gameObject.transform.Rotate(new Vector3(0f, 1.0f, 0f), Space.World);
        }
        else
        {
            camLight.gameObject.transform.Rotate(new Vector3(0f, 1.0f, 0f), Space.World);
        }
        yield return new WaitForSeconds(0.01f);
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    IEnumerator RotateBulbYMinusCoroutine()
    {
        if (m_isCustomMode == true)
        {
            customLight.gameObject.transform.Rotate(new Vector3(0f, -1.0f, 0f), Space.World);
        }
        else
        {
            camLight.gameObject.transform.Rotate(new Vector3(0f, -1.0f, 0f), Space.World);
        }
        yield return new WaitForSeconds(0.01f);
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    IEnumerator RotateBulbZPlusCoroutine()
    {
        if (m_isCustomMode == true)
        {
            customLight.gameObject.transform.Rotate(new Vector3(0f, 0f, 1.0f), Space.World);
        }
        else
        {
            camLight.gameObject.transform.Rotate(new Vector3(0f, 0f, 1.0f), Space.World);
        }
        yield return new WaitForSeconds(0.01f);
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    IEnumerator RotateBulbZMinusCoroutine()
    {
        if (m_isCustomMode == true)
        {
            customLight.gameObject.transform.Rotate(new Vector3(0f, 0f, -1.0f), Space.World);
        }
        else
        {
            camLight.gameObject.transform.Rotate(new Vector3(0f, 0f, -1.0f), Space.World);
        }
        yield return new WaitForSeconds(0.01f);
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    public void SetIntensity(float value)
    {
        camLight.intensity = value;
        customLight.intensity = value;
    }

    public bool SetCustomlight(bool isOn)
    {
        if (isOn == true)
        {
            customLight.gameObject.SetActive(true);
            camLight.gameObject.SetActive(false);
        }   
        else
        {
            camLight.gameObject.SetActive(true);
            customLight.gameObject.SetActive(false);
        }
        UpdateBulbRotation();
        Debug.Log("light mode is changed");
        return isOn;
    }
}
