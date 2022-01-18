using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
public class PalletManagerForPro : MonoBehaviour
{
    #region singleton
    static public PalletManagerForPro Instance;
    private void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
    }
    #endregion

    //---------init-------------
    [HideInInspector]public CMDState initState = CMDState.BUSY;
    Coroutine coroutine;
    //--------------------------

    public Image cursorImage;
    [HideInInspector]
    public int palletId = 0;
    public PalletSelectButtonForPro[] palletSelectButtons = new PalletSelectButtonForPro[150];
    private int selectButtonIdx;

    [SerializeField] ColorPicker colorPicker;        
    private bool isColorPickerOn = false;
    
    private void Start()
    {
        StartCoroutine(InitSetting());
    }
    IEnumerator InitSetting()
    {
        initState = CMDState.BUSY;

        coroutine = StartCoroutine(SetPalletCoroutine());
        yield return new WaitUntil(() => coroutine == null);
        
        coroutine = StartCoroutine(DeactiveColorPicker());
        yield return new WaitUntil(() => coroutine == null);

        initState = CMDState.IDLE;
        if (gameObject.activeSelf) gameObject.SetActive(false);
        //Debug.Log("Pallet Manager For Pro : InitSetting() completed !!");
    }
    IEnumerator SetPalletCoroutine()
    {
        //yield return new WaitForSeconds(0.3f);
        yield return new WaitForEndOfFrame();
        // 시간 지연을 준 이유: start() 에서 foreach를 바로돌릴경우 다른 클래스의 start() 와 충돌이 일어나는것 같다.
        // 시간지연후 코루틴을 돌리면 에러없이 잘됨. 바로 Pallet.start()에서 foreach 도는 도중에 PalletSelectButton.start()가  호출되어버리는듯.
        int i = 0;
        foreach (Material sub in PalletInventory.Instance.list_Material)
        {
            if (palletSelectButtons[i] != null)
            {
                palletSelectButtons[i].SetMaterial(sub);
                i++;
            }
            yield return null;
        }
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    public void SetPallet()
    {
        int i = 0;
        foreach (Material sub in PalletInventory.Instance.list_Material)
        {
            if (palletSelectButtons[i] != null)
            {
                palletSelectButtons[i].SetMaterial(sub);
                i++;
            }
        }
        CloseColorPicker();
    }
    IEnumerator DeactiveColorPicker()
    {
        yield return new WaitForSeconds(0.01f);
        colorPicker.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        colorPicker.gameObject.SetActive(false);

        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }

    /// <summary>
    /// pallet buttons
    /// </summary>
    // toggle color picker
   

    public void SetSelectButtonIdx(int idx)
    {
        selectButtonIdx = idx;

        // if color picker already exist, switch previous color picker
        if (isColorPickerOn==true)
        {
            ColorPicker.instance.CCancel();
            palletSelectButtons[selectButtonIdx].ChooseColorButtonClick();
            if (isColorPickerOn == false)
            {
                CloseColorPicker();
            }
        }
        else
        {
            ColorPicker.instance.CDone();
            palletSelectButtons[selectButtonIdx].ChooseColorButtonClick();
            if (isColorPickerOn == false)
            {
                CloseColorPicker();
            }

        }
        palletSelectButtons[selectButtonIdx].SetCursorMaterial();
    }

    public void CloseColorPicker()
    {
        ColorPicker.instance.CCancel();
    }
    public void ChangeColorAndCloseColorPicker()
    {
        ColorPicker.instance.CDone();
    }


    public void DisplayCursor()
    {
        cursorImage.transform.position = palletSelectButtons[selectButtonIdx].transform.position;
    }

    public List<float4> GetColorData()
    {
        List<float4> list_ColorValue = new List<float4>();
        float4 tmpColorValue;
        foreach (var item in palletSelectButtons)
        {
            if (item != null)
            {
                Color tmpColor = item.GetMaterial().color;
                tmpColorValue = new float4(tmpColor.r, tmpColor.g, tmpColor.b, tmpColor.a);
                list_ColorValue.Add(tmpColorValue);
            }
        }
        return list_ColorValue;
    }

    public PalletData GetPalletData()
    {
        PalletData palletData = new PalletData();
        List<int> list_matNum = new List<int>();
        List<float4> list_colorValue = new List<float4>();
        float4 tmpColorValue;
        foreach (var item in palletSelectButtons)
        {
            if (item != null)
            {
                list_matNum.Add(item.palletSelectButtonNumber);
                Color tmpColor = item.GetMaterial().color;
                tmpColorValue = new float4(tmpColor.r, tmpColor.g, tmpColor.b, tmpColor.a);
                list_colorValue.Add(tmpColorValue);
            }
        }
        palletData.palletColorData.list_color = list_colorValue;
        return palletData;
    }

    public void SetPalletWithJsonData(PalletData data)
    {
        int idx = 0;
        int palletNum = palletSelectButtons.Length;
        foreach (var colorValue in data.palletColorData.list_color)
        {
            Color tmpColor = new Color(colorValue.x,
                                       colorValue.y,
                                       colorValue.z,
                                       colorValue.w);
            if (idx < palletNum)
                palletSelectButtons[idx].SetColor(tmpColor);
            idx++;
        }
    }


    /// <summary>
    /// Color picking pallet
    /// </summary>
    public void OnColorEditButtonClicked()
    {
        if (coroutine != null) return;

        if (isColorPickerOn == false)
        {   
            colorPicker.gameObject.SetActive(true);
            isColorPickerOn = true;
        }
        else
        {   
            colorPicker.gameObject.SetActive(false);
            isColorPickerOn = false;
        }
    }
    public void OnDoneButtonClicked()
    {
        if (coroutine != null) return;

        EditorUIManager.Instance.cubeDesigneInstance.ResetCursorMaterial();
        ChangeColorAndCloseColorPicker();
        if (isColorPickerOn == true)
        {
            colorPicker.gameObject.SetActive(false);
            isColorPickerOn = false;
        }
    }
    public void OnCancelButtonClicked()
    {
        if (coroutine != null) return;

        CloseColorPicker();
        if (isColorPickerOn == true)
        {
            colorPicker.gameObject.SetActive(false);
            isColorPickerOn =false;
        }
    }

    public void OnSaveButtonClicked()
    {
        PalletData currentPalletData = new PalletData(true);
        string savePath = "C:/Unity/userData/Pallet/" + currentPalletData.id.ToString() + ".json";
        System.IO.File.WriteAllText(savePath, JsonUtility.ToJson(currentPalletData));
    }

    public void OnUseButtonClicked(int palletNumber)
    {
        string targetPath = "C:/Unity/userData/Pallet/" + palletNumber + ".json";
        string json = System.IO.File.ReadAllText(targetPath);
        PalletData data = JsonUtility.FromJson<PalletData>(json);
        SetPalletWithJsonData(data);
    }
}
