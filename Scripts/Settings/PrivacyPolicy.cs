using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicy : MonoBehaviour
{
    public string privacyPolicyURL = "https://1drv.ms/t/s!AonVcoQQ6G8FcPzeuPOuORSsMN4?e=oUf4fV";

    public void OpenLink()
    {
        Application.OpenURL(privacyPolicyURL);
    }
}
