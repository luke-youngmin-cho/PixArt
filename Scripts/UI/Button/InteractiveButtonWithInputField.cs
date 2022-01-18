using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InteractiveButtonWithInputField : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] InputField inputField;
        
    // Update is called once per frame
    void Update()
    {
        if(inputField.text == "")
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
}
