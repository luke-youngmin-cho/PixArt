using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
public class PalletManager : MonoBehaviour
{
    #region singleton
    static public PalletManager Instance;
    private void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
    }
    #endregion
    //---------init-------------
    public CMDState initState = CMDState.BUSY;
    Coroutine coroutine;
    //--------------------------

    public Image cursorImage;
    [HideInInspector]
    public int palletId = 0;
    public PalletSelectButton[] palletSelectButtons = new PalletSelectButton[99];
    public List<MaterialSelectButton> materialSelectButton_List= new List<MaterialSelectButton>();
    [HideInInspector]
    public Material selectedMaterial;
    private int selectButtonIdx;

    [SerializeField] RectTransform colorPickingPalletRotatingPoint;
    [SerializeField] GameObject colorSelectingPallet;
    [SerializeField] GameObject colorPickingPallet;
    [SerializeField] ColorPicker colorPicker;        
    private bool isColorPickingPalletOn = false;
    
    private void Start()
    {
        StartCoroutine(InitSetting());
    }
    IEnumerator InitSetting()
    {
        initState = CMDState.BUSY;

        coroutine = StartCoroutine(SetPallet());
        yield return new WaitUntil(() => coroutine == null);

        coroutine = StartCoroutine(DeactiveColorPicker());
        yield return new WaitUntil(() => coroutine == null);

        initState = CMDState.IDLE;
        //Debug.Log("PalletManager : InitSetting() completed !!");

    }
    IEnumerator SetPallet()
    {
        yield return new WaitForSeconds(0.2f);
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
        }
        StopCoroutine(coroutine);
        coroutine = null;
        yield return null;
    }
    IEnumerator DeactiveColorPicker()
    {
        yield return new WaitForSeconds(0.1f);
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
        if (isColorPickingPalletOn==true)
        {
            colorPicker.CCancel();
            palletSelectButtons[selectButtonIdx].ChooseColorButtonClick();
            if (isColorPickingPalletOn == false)
            {
                CloseColorPicker();
            }

            CameraHandler.instance.DisableHandling();
        }
        else
        {
            colorPicker.CDone();
            palletSelectButtons[selectButtonIdx].ChooseColorButtonClick();
            if (isColorPickingPalletOn == false)
            {
                CloseColorPicker();
            }

        }
        palletSelectButtons[selectButtonIdx].SetCursorMaterial();
        selectedMaterial = palletSelectButtons[selectButtonIdx].GetMaterial();
    }

    public void CloseColorPicker()
    {
        colorPicker.CCancel();
        CameraHandler.instance.EnableHandling();
    }
    public void ChangeColorAndCloseColorPicker()
    {
        colorPicker.CDone();
        CameraHandler.instance.EnableHandling();
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

        if (isColorPickingPalletOn == false)
        {   
            colorPicker.gameObject.SetActive(true);
            isColorPickingPalletOn = true;
            CameraHandler.instance.DisableHandling();
            coroutine = StartCoroutine(ChangeScaleAndRotation(colorPickingPalletRotatingPoint, 2.0f, 90f));
        }
        else
        {   
            colorPicker.gameObject.SetActive(false);
            isColorPickingPalletOn = false;

            CameraHandler.instance.EnableHandling();
            coroutine = StartCoroutine(ChangeScaleAndRotation(colorPickingPalletRotatingPoint, 1.0f, 0f));
        }
    }
    public void OnDoneButtonClicked()
    {
        if (coroutine != null) return;

        ChangeColorAndCloseColorPicker();
        if (isColorPickingPalletOn == true)
        {
            colorPicker.gameObject.SetActive(false);
            coroutine = StartCoroutine(ChangeScaleAndRotation(colorPickingPalletRotatingPoint, 1.0f, 0f));
            isColorPickingPalletOn = false;
        }
    }
    public void OnCancelButtonClicked()
    {
        if (coroutine != null) return;

        CloseColorPicker();
        if (isColorPickingPalletOn == true)
        {
            colorPicker.gameObject.SetActive(false);
            coroutine = StartCoroutine(ChangeScaleAndRotation(colorPickingPalletRotatingPoint, 1.0f, 0f));
            isColorPickingPalletOn =false;
        }
    }

    IEnumerator RevealColorPickingPallet(Transform target, int scaleValue, int rotationValue)
    {
        Vector3 unitScale = new Vector3(0f, 0f, Mathf.Abs(target.localScale.z - scaleValue) / 10);
        Vector3 unitRotation = new Vector3(0f, 0f, rotationValue/10);
        Quaternion q = target.rotation;
        int count = 1;
        while (count <= 10)
        {
            target.localScale = unitScale*count;
            q.eulerAngles = unitRotation;
            target.rotation = q;
            count++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    IEnumerator HideColorPickingPallet(Transform target, float scaleValue, float rotationValue)
    {
        Vector3 unitScale = new Vector3(0f, 0f, (target.localScale.z - scaleValue) / 10);
        Vector3 unitRotation = new Vector3(0f, 0f, rotationValue / 10);
        Quaternion q = target.rotation;
        int count = 1;
        while (count <= 10)
        {
            target.localScale = target.localScale - unitScale * count;
            q.eulerAngles = unitRotation;
            target.rotation = q;
            count++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    IEnumerator ChangeScaleAndRotation(Transform target, float scaleValue, float angleValue)
    {
        Debug.Log("start changing");
        Vector3 currentScale = target.localScale;
        Vector3 currentAngle = target.rotation.eulerAngles;
        Vector3 unitScale    = new Vector3((currentScale.x - scaleValue) / 10,
                                           (currentScale.y - scaleValue) / 10,
                                           1f);
        Vector3 unitAngle    = new Vector3(0f,
                                           0f,
                                           (currentAngle.z - angleValue) / 10);
        Quaternion q = target.rotation;
        int count = 1;
        while (count <= 10)
        {
            target.localScale = new Vector3(currentScale.x - unitScale.x * count,
                                            currentScale.y - unitScale.y * count,
                                            1.0f);
            q.SetEulerRotation(new Vector3(Mathf.Deg2Rad * (currentAngle.x - unitAngle.x * count),
                                           Mathf.Deg2Rad * (currentAngle.y - unitAngle.y * count),
                                           Mathf.Deg2Rad * (currentAngle.z - unitAngle.z * count)));

            target.rotation = q;
            count++;
            yield return new WaitForSeconds(0.005f);
        }
        coroutine = null;
        yield return null;
    }
    public void SaveCurrentPallet(int id)
    {
        PalletData currentPalletData = new PalletData(true);
        currentPalletData.id = id;
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

    public bool isColorPickerOn()
    {
        bool isOn = false;
        if (colorPicker.gameObject.activeSelf == true)
            isOn = true;
        return isOn;
    }
}
