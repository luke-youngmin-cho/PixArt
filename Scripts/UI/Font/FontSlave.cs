using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FontSlave : MonoBehaviour
{
    void Start()
    {
        FontMaster.instance.Register(this);
        FontMaster.instance.RefreshFont();
    }
    public void ChangeFont(Font font)
    {
        this.gameObject.GetComponent<Text>().font = font;
    }
}
