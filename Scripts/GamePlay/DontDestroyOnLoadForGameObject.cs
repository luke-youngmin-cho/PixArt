using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadForGameObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
