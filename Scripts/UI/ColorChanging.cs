using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanging : MonoBehaviour
{
    public Image image;
    [SerializeField] float min;
    [SerializeField] float max;
    float num1;
    float num2;
    float num3;
    e_FSM FSM;
    private void Start()
    {
        num1 = min;
        num2 = min;
        num3 = min;
    }
    void Update()
    {
        switch (FSM)
        {
            case e_FSM.rp:
                num1 += 1f;
                if (num1 >= max) FSM = e_FSM.gp;
                break;
            case e_FSM.gp:
                num2 += 1f;
                if (num2 >= max) FSM = e_FSM.bp;
                break;
            case e_FSM.bp:
                num3 += 1f;
                if (num3 >= max) FSM = e_FSM.rm;
                break;
            case e_FSM.rm:
                num1 -= 1f;
                if (num1 <= min) FSM = e_FSM.gm;
                break;
            case e_FSM.gm:
                num2 -= 1f;
                if (num2 <= min) FSM = e_FSM.bm;
                break;
            case e_FSM.bm:
                num3 -= 1f;
                if (num3 <= min) FSM = e_FSM.rp;
                break;
            default:
                break;
        }
        image.GetComponent<Image>().color = new Color(num1/255f,num2/255f,num3/255f);
    }
}

enum e_FSM
{
    rp,
    gp,
    bp,
    rm,
    gm,
    bm
}