using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEngine.UI;
using Unity.Mathematics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace DOTS
{
    public class DataManager : MonoBehaviour
    {
        static public DataManager instance;
        private void Awake()
        {
            if (instance == null) instance = this;
        }
        //---------init-------------
        public CMDState initState = CMDState.BUSY;
        Coroutine coroutine;
        //--------------------------

        private void Start()
        {
            StartCoroutine(InitSetting());
        }

        IEnumerator InitSetting()
        {
            initState = CMDState.BUSY;
            SettingsManager.instance.LoadSavedData();
            yield return new WaitUntil(() => coroutine == null);
            //Debug.Log("DataManger : InitSetting() Progress -> coroutine is null");
            palletManagerObj.SetActive(true);
            yield return new WaitUntil(() => (palletManagerObj.activeSelf));
            //Debug.Log("PalletManager Obj Activated");
            yield return new WaitUntil(() => CheckPalletManagerInitStateIDLE() == true);
            //Debug.Log("DataManger : InitSetting() Progress -> Pallet Manager init setting has benn finished");
            yield return new WaitUntil(() => LoadUnitMaterialData() == true);
            //Debug.Log("DataManger : InitSetting() Progress -> Load unit material data completed");
            initState = CMDState.IDLE;
            //Debug.Log("DataManager : InitSetting() completed !!");
        }
        private bool CheckPalletManagerInitStateIDLE()
        {
            CMDState state = CMDState.BUSY;
            
            //Debug.Log($"GameManger mode : {GameManager.instance._mode}");
            if ((GameManager.instance._mode == e_Mode.Kids) & (PalletManager.Instance != null))
            {
                state = PalletManager.Instance.initState;
            }

            if ((GameManager.instance._mode == e_Mode.Pro) & (PalletManagerForPro.Instance != null))
            {
                state = PalletManagerForPro.Instance.initState;
            }

            if (state == CMDState.IDLE) return true;
            else return false;
        }
        ///  Cube Design Data
        //==================================================
        [HideInInspector] public string unitMaterialCategory;
        [HideInInspector] public Material unitMaterial;
        private CubeDesign currentCubeDesignInstance;
        private CubeDesignData currentCubeDesigneData;
        public Coroutine SavingCubeDesignDataCoroutine;
        ///saved design info
        //===================================================
        [HideInInspector] string LatestSavedCubeDesignName = "";
        SavedDesignsInfo[] allSavedDesignInfo;
        public GameObject openPanel;
        private List<GameObject> List_saveDesingUI = new List<GameObject>();

        [HideInInspector] public CubeDesign cubeDesign;
        [HideInInspector] public List<CubeDesign> list_cubeDesign = new List<CubeDesign>();
        [SerializeField] Transform cubeDesignTransform;

        Coroutine spawnBricksCoroutine;

        // UI
        //====================================================
        [SerializeField] private Canvas editorUIManager;
        [SerializeField] private GameObject initPanel;
        [SerializeField] private GameObject palletManagerObj;
        [SerializeField] private GameObject pleaseWaitCanvas;

        // Scene controller
        [SerializeField] private bl_SceneLoader sceneLoader;
        public bool LoadUnitMaterialData()
        {
            bool completed = false;
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if ((currentSceneName == "CubeEditorForKids") & (PalletManager.Instance != null))
            {
                unitMaterialCategory = "PalletSelectButtonForKids";
                unitMaterial = PalletManager.Instance.palletSelectButtons[0].GetMaterial();
                completed = true;
            }
            else if ((currentSceneName == "CubeEditorForPro") & (PalletManagerForPro.Instance != null))
            {
                unitMaterialCategory = "PalletSelectButtonForPro";
                unitMaterial = PalletManagerForPro.Instance.palletSelectButtons[0].GetMaterial();
                completed = true;
            }
            return completed;
        }

        public void RefreshDirectory()
        {
#if Editor
        AssetDatabase.Refresh();
#endif
        }
        //=======================================================================================
        // CubeDesign Data
        //=======================================================================================
        public List<CubeDesignData> GetAllCubeDesignData()
        {
            CubeDesignData data;
            List<CubeDesignData> list_data = new List<CubeDesignData>();
            string json;
            string targetPath = "C:/Unity/userData/CubeDesign";
            foreach (var dataPath in System.IO.Directory.GetFiles(targetPath))
            {
                json = System.IO.File.ReadAllText(dataPath);
                data = JsonUtility.FromJson<CubeDesignData>(json);
                list_data.Add(data);
            }
            return list_data;
        }
        // call this method only in CubeDesign Editor Scene. CubeDesignData() gets data of CubeDesign object which is opened in CubeDesign Scene.
        public void SaveCubeDesignData()
        {
            Debug.Log($"try to save cube design data name with {LatestSavedCubeDesignName}");
            if(LatestSavedCubeDesignName != "")
            {
                if (FindCubeDesignDataByName(LatestSavedCubeDesignName) == false) return;
                if (currentCubeDesignInstance.IsSaved() == true) return;
                SaveCubeDesignData(LatestSavedCubeDesignName);
            }
        }
        public void SaveCubeDesignData(InputField saveNameInputField)
        {
            StartCoroutine(SaveCubeDesignDataCoroutine(saveNameInputField.text));
            RefreshDirectory();
            saveNameInputField.text = "";
        }
        private void SaveCubeDesignData(string fileName)
        {
            StartCoroutine(SaveCubeDesignDataCoroutine(fileName));
            RefreshDirectory();
        }
        IEnumerator SaveCubeDesignDataCoroutine(string fileName)
        {
            
            // Restrict user controlls
            //------------------------------------------------------------------------------------------
            LatestSavedCubeDesignName = fileName;
            CubeDesignRayCast.instance.enable = false;
            CameraHandler.instance.DisableHandling();
            EditorUIManager.Instance.gameObject.SetActive(false); // disable all UI
            editorUIManager.enabled = false; // Disable UI canvas;

            

            //string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
            //string pathToSave = Application.persistentDataPath + "/" + "DesignIcon_" + fileName + "%A%Z%" + timeStamp + ".png";
            string iconPath = Application.persistentDataPath + "/" + "DesignIcon_" + fileName + ".png";
            CubeDesignGrid.instance.GridOff();
            yield return null;

            CameraHandler.instance.SetCameraDefaultSettings(); // Set camera default angle.
            yield return null;

            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            yield return null;

            EditorUIManager.Instance.cubeDesigneInstance.DisableCursor(); // disable cursor
            yield return null;

            yield return new WaitForEndOfFrame(); // CaptureScreenshotAsTexture() must called at the end of frame.
            tex = ScreenCapture.CaptureScreenshotAsTexture();
            editorUIManager.enabled = true;
            CubeDesignGrid.instance.GridOn();
            float width = openPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;
            float height = openPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.height;
            tex = TextureTool.ResampleAndCrop(tex, (int)width * 10, (int)height * 10);
            yield return null;
            byte[] bytes = tex.EncodeToPNG();
            yield return null;
            File.WriteAllBytes(iconPath, bytes);
            yield return null;

            pleaseWaitCanvas.SetActive(true);
            yield return null;
            // get data async
            //------------------------------------------------------------------------------------------
            //var asyncTask = Task.Run(currentCubeDesignInstance.GetBricksInfoAsync); // Update CubeDesign Info
            //yield return new WaitUntil(() => asyncTask.IsCompleted);

            SavingCubeDesignDataCoroutine = StartCoroutine(currentCubeDesignInstance.GetBricksInfoCoroutine());
            yield return new WaitUntil(() => SavingCubeDesignDataCoroutine == null);
            Debug.Log("Data Manager - Get Bricks Info finished");
            currentCubeDesigneData = new CubeDesignData(true);
            //var asyncTask = Task.Run(CubeDesignDataUpdateAsync); // Apply CubeDesign info to CubeDesignData
            //yield return new WaitUntil(() => asyncTask.IsCompleted);

            SavingCubeDesignDataCoroutine = StartCoroutine(CubeDesignDataUpdateCoroutine());
            yield return new WaitUntil(() => SavingCubeDesignDataCoroutine == null);
            Debug.Log("Data Manager - Update Cube Design Data Finished");
            yield return null;

            //string savePath = "C:/Unity/userData/CubeDesign/" + fileName + ".json";
            string jsonPath = Application.persistentDataPath + "/" + "Design_" + fileName + ".json";
            System.IO.File.WriteAllText(jsonPath, JsonUtility.ToJson(currentCubeDesigneData));

            EditorUIManager.Instance.gameObject.SetActive(true); // enable all UI
            currentCubeDesignInstance.EnableCursor(); // enable cursor

            CameraHandler.instance.EnableHandling();
            CubeDesignRayCast.instance.enable = true;
            currentCubeDesignInstance.SetSavedFlag(true);
            pleaseWaitCanvas.SetActive(false);
            yield return null;
        }
        async Task CubeDesignDataUpdateAsync()
        {
            if (currentCubeDesignInstance == null) return;

            await Task.Run(() =>
            {
                List<BrickInfo> list_bricksInfo = currentCubeDesignInstance.List_BricksInfo;

                currentCubeDesigneData.cubeDesingName = LatestSavedCubeDesignName;
                currentCubeDesigneData.resolution = currentCubeDesignInstance.resolution;

                //bricks data
                currentCubeDesigneData.bricksData.list_BrickInfo_id = new List<int>();
                currentCubeDesigneData.bricksData.list_BrickInfo_isExist = new List<bool>();
                currentCubeDesigneData.bricksData.list_BrickInfo_meshName = new List<string>();
                currentCubeDesigneData.bricksData.list_BrickInfo_matCategory = new List<string>();
                currentCubeDesigneData.bricksData.list_BrickInfo_matName = new List<string>();
                currentCubeDesigneData.bricksData.list_BrickInfo_rotationValue = new List<float3>();
                currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor = new List<float>();

                foreach (var item in list_bricksInfo)
                {
                    currentCubeDesigneData.bricksData.list_BrickInfo_id.Add(item._id);
                    currentCubeDesigneData.bricksData.list_BrickInfo_isExist.Add(item._isExist);
                    currentCubeDesigneData.bricksData.list_BrickInfo_meshName.Add(item._meshName);
                    currentCubeDesigneData.bricksData.list_BrickInfo_matCategory.Add(item._matCategory);
                    currentCubeDesigneData.bricksData.list_BrickInfo_matName.Add(item._matName);
                    currentCubeDesigneData.bricksData.list_BrickInfo_rotationValue.Add(item._rotationValue);
                    currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor.Add(item._scaleFactor);
                }
                currentCubeDesigneData.isSuccessfullyConstructed = true;
            });
        }
        
        IEnumerator CubeDesignDataUpdateCoroutine()
        {
            int tmpIdx = 0;
            if(currentCubeDesignInstance != null)
            {
                List<BrickInfo> list_bricksInfo = currentCubeDesignInstance.List_BricksInfo;

                currentCubeDesigneData.cubeDesingName = LatestSavedCubeDesignName;
                currentCubeDesigneData.resolution = currentCubeDesignInstance.resolution;

                //bricks data
                currentCubeDesigneData.bricksData.list_BrickInfo_id = new List<int>();
                currentCubeDesigneData.bricksData.list_BrickInfo_isExist = new List<bool>();
                currentCubeDesigneData.bricksData.list_BrickInfo_meshName = new List<string>();
                currentCubeDesigneData.bricksData.list_BrickInfo_matCategory = new List<string>();
                currentCubeDesigneData.bricksData.list_BrickInfo_matName = new List<string>();
                currentCubeDesigneData.bricksData.list_BrickInfo_rotationValue = new List<float3>();
                currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor = new List<float>();

                foreach (var item in list_bricksInfo)
                {
                    currentCubeDesigneData.bricksData.list_BrickInfo_id.Add(item._id);
                    currentCubeDesigneData.bricksData.list_BrickInfo_isExist.Add(item._isExist);
                    currentCubeDesigneData.bricksData.list_BrickInfo_meshName.Add(item._meshName);
                    currentCubeDesigneData.bricksData.list_BrickInfo_matCategory.Add(item._matCategory);
                    currentCubeDesigneData.bricksData.list_BrickInfo_matName.Add(item._matName);
                    currentCubeDesigneData.bricksData.list_BrickInfo_rotationValue.Add(item._rotationValue);
                    currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor.Add(item._scaleFactor);
                    
                    if(tmpIdx % 2000 == 0)
                        yield return null;
                    tmpIdx++;
                }
                currentCubeDesigneData.isSuccessfullyConstructed = true;
                SavingCubeDesignDataCoroutine = null;
            }
        }

        // call this method only in CubeDesign Scene.
        public void OpenCubeDesignData(string fileName)
        {
            StartCoroutine(CreateCubeDesingWithJsonData(fileName));
            OpenPanel.instance.CloseThis();
            // close init panels
            if (initPanel != null) initPanel.SetActive(false);
        }
        public void OpenCubeDesignData(Text fileName)
        {
            StartCoroutine(CreateCubeDesingWithJsonData(fileName.text));
            OpenPanel.instance.CloseThis();
            // close init panels;
            if (initPanel != null) initPanel.SetActive(false);
        }
        public bool IsLatestSavedCubeDesignExist()
        {
            bool isExist = false;
            if (LatestSavedCubeDesignName != "")
                isExist = true;
            return isExist;
        }
        private bool FindCubeDesignDataByName(string fileName)
        {
            bool isExist = false;
            if (fileName == "") return isExist;
            string targetPath = Application.persistentDataPath + "/" + "Design_" + fileName + ".json";
            string json = System.IO.File.ReadAllText(targetPath);
            if (json != "")
                isExist = true;
            return isExist;
        }
        private string GetCubeDesignJsonDataByName(string fileName)
        {
            string targetPath = Application.persistentDataPath + "/" + "Design_" + fileName + ".json";
            string json = System.IO.File.ReadAllText(targetPath);
            return json;
        }

        public void DeleteCubeDesignData(Text fileName)
        {
            string jsonPath = Application.persistentDataPath + "/" + "Design_" + fileName.text + ".json";
            string iconPath = Application.persistentDataPath + "/" + "DesignIcon_" + fileName.text + ".png";
            System.IO.File.Delete(jsonPath);
            System.IO.File.Delete(iconPath);
            RefreshDirectory();
            Debug.Log($"CubeDesign Data {fileName.text} has been destroyed");
        }

        IEnumerator CreateCubeDesingWithJsonData(string fileName)
        {
            string json = GetCubeDesignJsonDataByName(fileName);
            currentCubeDesigneData = JsonUtility.FromJson<CubeDesignData>(json);
            yield return null;
            yield return null;
            yield return null;

            if(currentCubeDesigneData == null)
            {
                Debug.LogError("DataManager : Invalid resolution. failed to open cube design");
                sceneLoader.LoadLevel(SceneManager.GetActiveScene().name);
            }
            else
            {
                int resolution = currentCubeDesigneData.resolution;
                EditorUIManager.Instance.CreateCubeDesign(resolution);
                currentCubeDesignInstance = EditorUIManager.Instance.GetCurrentCubeDesign();
                yield return new WaitUntil(() => currentCubeDesignInstance.initState == CMDState.IDLE);

                //var asyncTask = Task.Run(SpawnBricksWithDataAsync);
                //yield return new WaitUntil(() => asyncTask.IsCompleted);

                spawnBricksCoroutine = StartCoroutine(SpawnBricksWithDataCoroutine());
                yield return new WaitUntil(() => spawnBricksCoroutine == null);

                currentCubeDesignInstance.MoveCursorTo(0);
                list_cubeDesign.Add(currentCubeDesignInstance);
                currentCubeDesignInstance.SetSavedFlag(true);
                LatestSavedCubeDesignName = currentCubeDesigneData.cubeDesingName;
            }
        }

        async Task SpawnBricksWithDataAsync()
        {
            string tmpMeshName;
            string tmpMatCategory;
            string tmpMatName;
            float3 tmpRotationValue;
            float tmpScaleFactor;
            int tmpId;

            Mesh mesh;
            Material mat;

            bool isScaleFactorExist = false;

            await Task.Run(() =>
            {
                if (currentCubeDesigneData.bricksData.list_BrickInfo_id.Count == currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor.Count)
                    isScaleFactorExist = true;

                for (int i = 0; i < currentCubeDesigneData.bricksData.list_BrickInfo_id.Count; i++)
                {
                    if (currentCubeDesigneData.bricksData.list_BrickInfo_isExist[i] == true)
                    {
                        tmpId = currentCubeDesigneData.bricksData.list_BrickInfo_id[i];
                        tmpMeshName = currentCubeDesigneData.bricksData.list_BrickInfo_meshName[i];
                        tmpMatCategory = currentCubeDesigneData.bricksData.list_BrickInfo_matCategory[i];
                        tmpMatName = currentCubeDesigneData.bricksData.list_BrickInfo_matName[i];
                        tmpRotationValue = currentCubeDesigneData.bricksData.list_BrickInfo_rotationValue[i];
                        if (isScaleFactorExist == true)
                            tmpScaleFactor = currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor[i];
                        else
                            tmpScaleFactor = 1.0f;

                        // 워커스레드에서는 Unity API 에 접근할 수 없음
                        mesh = Resources.Load<Mesh>($"Meshs/{tmpMeshName}");
                        mat = Resources.Load<Material>($"Materials/{tmpMatCategory}/{tmpMatName}");
                        
                        currentCubeDesignInstance.PutBrick(tmpId, mesh, tmpMatCategory, mat, tmpRotationValue, tmpScaleFactor);
                    }
                }
                currentCubeDesignInstance.DestroyCursor();
                currentCubeDesignInstance.CreateCursor();
            });
        }
        IEnumerator SpawnBricksWithDataCoroutine()
        {
            string tmpMeshName;
            string tmpMatCategory;
            string tmpMatName;
            float3 tmpRotationValue;
            float tmpScaleFactor;
            int tmpId;

            Mesh mesh;
            Material mat;

            bool isScaleFactorExist = false;

            if (currentCubeDesigneData.bricksData.list_BrickInfo_id.Count == currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor.Count)
                isScaleFactorExist = true;

            for (int i = 0; i < currentCubeDesigneData.bricksData.list_BrickInfo_id.Count; i++)
            {
                if (currentCubeDesigneData.bricksData.list_BrickInfo_isExist[i] == true)
                {
                    tmpId = currentCubeDesigneData.bricksData.list_BrickInfo_id[i];
                    tmpMeshName = currentCubeDesigneData.bricksData.list_BrickInfo_meshName[i];
                    tmpMatCategory = currentCubeDesigneData.bricksData.list_BrickInfo_matCategory[i];
                    tmpMatName = currentCubeDesigneData.bricksData.list_BrickInfo_matName[i];
                    tmpRotationValue = currentCubeDesigneData.bricksData.list_BrickInfo_rotationValue[i];
                    if (isScaleFactorExist == true)
                        tmpScaleFactor = currentCubeDesigneData.bricksData.list_BrickInfo_scaleFactor[i];
                    else
                        tmpScaleFactor = 1.0f;
                    mesh = Resources.Load<Mesh>($"Meshs/{tmpMeshName}");
                    mat = Resources.Load<Material>($"Materials/{tmpMatCategory}/{tmpMatName}");
                    //Debug.Log($" try to get material from / {tmpMatCategory}/{tmpMatName}");
                    //Debug.Log(tmpId);
                    //Debug.Log(mesh.name);
                    //Debug.Log(mat.name);
                    //Debug.Log($"scaleFactor : {tmpScaleFactor}");
                    currentCubeDesignInstance.PutBrick(tmpId, mesh, tmpMatCategory, mat, tmpRotationValue, tmpScaleFactor);
                    if(i%100 == 0)
                        yield return new WaitForEndOfFrame();
                }
            }
            currentCubeDesignInstance.DestroyCursor();
            currentCubeDesignInstance.CreateCursor();
            spawnBricksCoroutine = null;
        }
        public void SpawnBricksWithData(CubeDesignData data)
        {
            string tmpMeshName;
            string tmpMatCategory;
            string tmpMatName;
            float3 tmpRotationValue;
            float tmpScaleFactor;
            int tmpId;

            Mesh mesh;
            Material mat;

            bool isScaleFactorExist = false;

            if (data.bricksData.list_BrickInfo_id.Count == data.bricksData.list_BrickInfo_scaleFactor.Count)
                isScaleFactorExist = true;


            for (int i = 0; i < data.bricksData.list_BrickInfo_id.Count; i++)
            {
                if (data.bricksData.list_BrickInfo_isExist[i] == true)
                {
                    tmpId = data.bricksData.list_BrickInfo_id[i];
                    tmpMeshName = data.bricksData.list_BrickInfo_meshName[i];
                    tmpMatCategory = data.bricksData.list_BrickInfo_matCategory[i];
                    tmpMatName = data.bricksData.list_BrickInfo_matName[i];
                    tmpRotationValue = data.bricksData.list_BrickInfo_rotationValue[i];
                    if (isScaleFactorExist == true)
                        tmpScaleFactor = data.bricksData.list_BrickInfo_scaleFactor[i];
                    else
                        tmpScaleFactor = 1.0f;
                    mesh = Resources.Load<Mesh>($"Meshs/{tmpMeshName}");
                    mat = Resources.Load<Material>($"Materials/{tmpMatCategory}/{tmpMatName}");

                    //Debug.Log($" try to get material from / {tmpMatCategory}/{tmpMatName}");
                    //Debug.Log(tmpId);
                    //Debug.Log(mesh.name);
                    //Debug.Log(mat.name);
                    //Debug.Log($"scaleFactor : {tmpScaleFactor}");
                    currentCubeDesignInstance.PutBrick(tmpId, mesh, tmpMatCategory, mat, tmpRotationValue, tmpScaleFactor);
                }
            }
            currentCubeDesignInstance.DestroyCursor();
            currentCubeDesignInstance.CreateCursor();
        }

        public IEnumerator GetSavedDesignInfo()
        {
            float size = openPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
            yield return null;

            int totalFileNumber = dir.GetFiles("DesignIcon_*").Length;
            FileInfo[] info = new FileInfo[totalFileNumber];
            info = dir.GetFiles("DesignIcon_*");
            yield return null;

            string tmpName = "";

            allSavedDesignInfo = new SavedDesignsInfo[totalFileNumber];
            for (int i = 0; i < info.Length; i++)
            {
                Byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/" + info[i].Name);
                yield return null;

                Texture2D texture = new Texture2D((int)size, (int)size);//, TextureFormat.RGBA32, false);
                texture.LoadImage(bytes);
                yield return null;

                allSavedDesignInfo[i].Icon = texture;
                //tmpName = info[i].Name.Remove(info[i].Name.IndexOf("%A%Z%"));
                tmpName = info[i].Name.Remove(info[i].Name.IndexOf(".png"));
                allSavedDesignInfo[i].name = tmpName.Replace("DesignIcon_", "");
                //Debug.Log($"Get Saved design info {allSavedDesignInfo[i].name}");
            }

            //Texture2D tx = w.texture;
            //allSavedDesignInfo[i].Icon = Sprite.Create(tx, new Rect(0f, 0f, tx.width, tx.height), Vector2.zero, 10f);

            DrawUIForSavedDesign();
            yield return null;
        }
        void DrawUIForSavedDesign()
        {
            Transform contentTransform = openPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            Transform itemTransform = openPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            float size = itemTransform.GetChild(0).GetComponent<RectTransform>().rect.width;
            GameObject g;

            int N = allSavedDesignInfo.Length;

            for (int i = 0; i < N; i++)
            {
                //Debug.Log($"UI {i} Drew");
                g = Instantiate(itemTransform.gameObject, contentTransform);

                Texture2D tex = new Texture2D((int)size, (int)size, TextureFormat.RGBA32, false);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                tex = allSavedDesignInfo[i].Icon;
                /*g.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(tex,
                                                                                     itemTransform.GetChild(0).GetComponent<RectTransform>().rect,
                                                                                     pivot);*/
                g.transform.GetChild(0).GetComponent<RawImage>().texture = tex;
                g.transform.GetChild(1).GetComponent<Text>().text = allSavedDesignInfo[i].name;
                g.transform.GetChild(2).GetComponent<Text>().text = allSavedDesignInfo[i].description;

                //g.transform.GetChild(3).GetComponent<Button>().AddEventListener(i, SavedDesignClicked);
                g.transform.Translate(new Vector3(10f, 0f, g.GetComponent<RectTransform>().rect.height));
                List_saveDesingUI.Add(g);
            }
            foreach (var go in List_saveDesingUI)
            {
                go.SetActive(true);
            }
            itemTransform.gameObject.SetActive(false);
            /*contentTransform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2( contentTransform.gameObject.GetComponent<RectTransform>().rect.width,
                                                                                               (N+1) * size);*/
            contentTransform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(contentTransform.gameObject.GetComponent<RectTransform>().sizeDelta.x,
                                                                                               (N + 1) * size);
        }
        public void RemoveUIForSavedDesign()
        {
            foreach (var go in List_saveDesingUI)
            {
                Destroy(go);
            }
            List_saveDesingUI.Clear();
        }

        void SavedDesignClicked(int itemIndex)
        {
            Debug.Log($" Tried to open [ {allSavedDesignInfo[itemIndex].name} ] ");
            OpenCubeDesignData(allSavedDesignInfo[itemIndex].name);
        }

        public void SetCurrentCubeDesign(CubeDesign cubeDesign)
        {
            currentCubeDesignInstance = cubeDesign;
        }

        public void ResetLatestSavedCubeDesignName()
        {
            LatestSavedCubeDesignName = "";
        }
        public string GetLatestSavedCubeDesignName()
        {
            return LatestSavedCubeDesignName;
        }
        //=======================================================================================
        // Pallet Data
        //=======================================================================================



        //=======================================================================================
        // Recipe Data
        //=======================================================================================
        //-----
        [SerializeField] GameObject recipesPanel;
        [SerializeField] GameObject recipePanel;
        [SerializeField] RawImage recipePage;
        RecipeInfo[] allSavedRecipesInfo;
        [SerializeField] Text currentRecipeNameText;
        [SerializeField] Text currentPageNumberText;
        [SerializeField] Text totalPageNumberText;
        private List<GameObject> List_RecipesUI = new List<GameObject>();
        int currentRecipeIdx = -1;
        int currentPageNumber = -1;
        //-----
        public void SaveRecipeData()
        {
            if(EditorUIManager.Instance.cubeDesigneInstance.IsSaved() == true)
                StartCoroutine(SaveRecipeDataCoroutine(LatestSavedCubeDesignName));
        }
        IEnumerator SaveRecipeDataCoroutine(string fileName)
        {
            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            float width = openPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;
            float height = openPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.height;
            byte[] bytes;
            RecipeData currentCubeDesignRecipe = new RecipeData();
            currentCubeDesignRecipe.name = fileName;
            string path = Application.persistentDataPath + "/Recipes/" + fileName;
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            // get cubedesign data and make recipe pages
            CubeDesignGrid.instance.GridOff();
            CubeDesignGrid.instance.GridOn(e_GridType.XZ);
            CameraHandler.instance.SetCameraDefaultSettings(); // Set camera default angle.
            EditorUIManager.Instance.gameObject.SetActive(false); // disable all UI
            currentCubeDesignInstance = EditorUIManager.Instance.GetCurrentCubeDesign();

            currentCubeDesignRecipe.resoltuion = currentCubeDesignInstance.resolution;
            currentCubeDesignRecipe.totalPageNumber = currentCubeDesignRecipe.resoltuion + 1;
            currentCubeDesignRecipe.recipePagesURL = new string[currentCubeDesignRecipe.totalPageNumber];
            currentCubeDesignInstance.DisableAllBricks();
            currentCubeDesignInstance.DisableCursor();
            currentCubeDesignInstance.MoveCursorTo(0);
            for (int i = 0; i < currentCubeDesignRecipe.totalPageNumber; i++)
            {
                currentCubeDesignInstance.EnableBricksOnXZPlane(i);
                yield return new WaitForEndOfFrame(); // CaptureScreenshotAsTexture() must called at the end of frame.
                tex = ScreenCapture.CaptureScreenshotAsTexture();
                tex = TextureTool.ResampleAndCrop(tex, (int)width, (int)height);
                bytes = tex.EncodeToPNG();

                currentCubeDesignRecipe.recipePagesURL[i] = "Recipes/" + fileName + "/" + i + ".png";
                File.WriteAllBytes(Application.persistentDataPath + "/"+ currentCubeDesignRecipe.recipePagesURL[i], bytes);
                currentCubeDesignInstance.MoveCursor_Yplus();
            }
            EditorUIManager.Instance.gameObject.SetActive(true); // enable all UI
            EditorUIManager.Instance.cubeDesigneInstance.EnableCursor(); // enable cursor

            System.IO.File.WriteAllText(path + "/info"+ ".json", JsonUtility.ToJson(currentCubeDesignRecipe));
        }
        public IEnumerator GetRecipesInfo()
        {
            float size = recipesPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.width;
                        
            string recipesPath = Application.dataPath + "/Resources/Recipes";
            string[] directories = new string[2];
                //GetRecipesDirectories(); 
                // Directory.GetDirectories(recipesPath, "*", SearchOption.AllDirectories);
            TextAsset textData;
            //Debug.Log($"recipes number : {directories.Length}");

            RecipeData[] recipeDataArray = new RecipeData[directories.Length];
            allSavedRecipesInfo = new RecipeInfo[directories.Length];

            for (int i = 0; i < directories.Length; i++)
            {
                //directories[i] = directories[i].Replace(@"\","/");
                //directories[i] = directories[i].Replace(recipesPath + "/", "");
                //Debug.Log($"{directories[i]}/info.json");

                //temp
                if (i == 0) 
                    directories[i] = "Chair";
                else if (i == 1)
                    directories[i] = "Watch";



                
                textData = Resources.Load("Recipes/" + directories[i] + "/info") as TextAsset;
                //Debug.Log(textData);
                RecipeData data = JsonUtility.FromJson<RecipeData>(textData.ToString());

                //Debug.Log(data);
                recipeDataArray[i] = data;
                // convert recipeData to recipeInfo
                allSavedRecipesInfo[i].name = recipeDataArray[i].name;
                allSavedRecipesInfo[i].resolution = recipeDataArray[i].resoltuion;
                allSavedRecipesInfo[i].totalPageNumber = recipeDataArray[i].totalPageNumber;

                //Debug.Log($"Directory : {"Recipes/" + directories[i].ToString()}");
                var pages = Resources.LoadAll<Texture2D>("Recipes/" + directories[i].ToString()).OrderBy(file => int.Parse(file.name));
                //Debug.Log($"recipe pages : {pages.Length}");

                allSavedRecipesInfo[i].recipePages = pages.ToArray();
                yield return new WaitForEndOfFrame();

                
                int iconIdx = 0;
                if(allSavedRecipesInfo[i].totalPageNumber > 0)
                {
                    iconIdx = allSavedRecipesInfo[i].totalPageNumber - 1;
                }
                //Debug.Log($"iconIdx : {iconIdx}");
                allSavedRecipesInfo[i].recipeIcon = allSavedRecipesInfo[i].recipePages[iconIdx];

            }
            yield return null;
            DrawRecipesUI();        
        }
        public static string[] GetRecipesDirectories()
        {
            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();
            // Add the root directory to the stack
            stack.Push(Application.dataPath);
            // While we have directories to process...
            while (stack.Count > 0)
            {
                // Grab a directory off the stack
                string currentDir = stack.Pop();
                try
                {
                    foreach (string dir in Directory.GetDirectories(currentDir))
                    {
                        if (Path.GetFileName(dir).Equals("Resources"))
                        {
                            // If one of the found directories is a Resources dir, add it to the result
                            result.Add(dir);
                        }
                        // Add directories at the current level into the stack
                        stack.Push(dir);
                        //Debug.Log($"path name : {dir}");
                    }
                }
                catch
                {
                    Debug.LogError("Directory " + currentDir + " couldn't be read from.");
                }
            }
            return result.ToArray();
        }
        void DrawRecipesUI()
        {
            Transform recipesContent = recipesPanel.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            GameObject buttonTemplate = recipesContent.GetChild(0).gameObject;
            GameObject g;

            int N = allSavedRecipesInfo.Length;

            for (int i = 0; i < N; i++)
            {
                g = Instantiate(buttonTemplate, recipesContent);
                g.transform.GetChild(0).GetComponent<RawImage>().texture = allSavedRecipesInfo[i].recipeIcon;
                g.transform.GetChild(1).GetComponent<Text>().text = allSavedRecipesInfo[i].name;
                g.transform.GetChild(2).GetComponent<Text>().text = "Size : " + allSavedRecipesInfo[i].resolution.ToString();

                g.GetComponent<Button>().AddEventListener(i, RecipeClicked);
                g.transform.Translate(new Vector3(10f, 0f, g.GetComponent<RectTransform>().rect.height));
                List_RecipesUI.Add(g);
            }
            foreach (var go in List_RecipesUI)
            {
                go.SetActive(true);
            }
            buttonTemplate.SetActive(false);
        }
        public void RemoveRecipesUI()
        {
            foreach (var go in List_RecipesUI)
            {
                Destroy(go);
            }
            List_RecipesUI.Clear();
        }
        void RecipeClicked(int idx)
        {
            RemoveRecipesUI();
            currentRecipeIdx = idx;
            currentPageNumber = 0;
            recipesPanel.SetActive(false);
            recipePanel.SetActive(true);
            recipePage.texture = allSavedRecipesInfo[idx].recipePages[currentPageNumber];

            currentRecipeNameText.text = allSavedRecipesInfo[idx].name + "(size : " + allSavedRecipesInfo[idx].resolution.ToString() + ")";
            currentPageNumberText.text = (currentPageNumber + 1).ToString();
            totalPageNumberText.text = allSavedRecipesInfo[idx].totalPageNumber.ToString();
        }

        public void GotoNextPage()
        {
            Texture2D page;
            int maxIdx = allSavedRecipesInfo[currentRecipeIdx].totalPageNumber - 1;
            if (maxIdx <= currentPageNumber)
            {
                currentPageNumber = maxIdx;
            }
            else
            {
                currentPageNumber++;
            }
            currentPageNumberText.text = (currentPageNumber + 1).ToString();
            page = allSavedRecipesInfo[currentRecipeIdx].recipePages[currentPageNumber];
            recipePage.texture = page;
        }
        public void GotoPreviousPage()
        {
            Texture2D page;
            int minIdx = 0;
            if (currentPageNumber <= 0)
            {
                currentPageNumber = minIdx;
            }
            else
            {
                currentPageNumber--;
            }
            currentPageNumberText.text = (currentPageNumber + 1).ToString();
            page = allSavedRecipesInfo[currentRecipeIdx].recipePages[currentPageNumber];
            recipePage.texture = page;
        }

    }
}
