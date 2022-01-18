using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActiveDeveloperMode : MonoBehaviour
{
    [SerializeField] GameObject ObjForDeveloper;
    private string PW = "2371717";
    public void CheckDeveloperPW(string input)
    {
        if(input == PW)
        {
            ObjForDeveloper.SetActive(true);
        }
    }
}
