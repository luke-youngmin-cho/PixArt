using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DOTS;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using System.Threading;

public enum e_GridType
{
    None,
    XY,
    XZ,
    YZ,
    XYZ,
    All,
    Outline
}

struct st_GridFlag
{
    public bool XY;
    public bool XZ;
    public bool YZ;
    public bool All;
    public bool OUTLINE;
}

namespace DOTS
{
    public class CubeDesignGrid : MonoBehaviour
    {
        public static CubeDesignGrid instance;
        [SerializeField] private bool activeEntityLines;

        /// Common variables
        [HideInInspector] public int HowManyGridLinesAre;
        private st_GridFlag gridFlag;
        private string _gridTypeName;

        /// Grid Plane
        [Header("Grid Plane")]
        [SerializeField] Transform cubeDesignTransform;
        [SerializeField] GameObject gridPlane;
        [SerializeField] Material gridPlaneMat;
        private GameObject gridPlaneXY;
        private GameObject gridPlaneYZ;
        private GameObject gridPlaneXZ;

        /// Entity lines
        [Header("Grid Entity Lines")]

        [HideInInspector] public e_GridType _gridType;

        public float gridThickness = 0.003f;

        private EntityManager entityManager;
        private List<Entity> list_entityALL = new List<Entity>();
        private List<Entity> list_entityXY = new List<Entity>();
        private List<Entity> list_entityXZ = new List<Entity>();
        private List<Entity> list_entityYZ = new List<Entity>();
        private World defaultWorld;
        private EntityArchetype archetype;

        [SerializeField] private Mesh unitMesh;
        [SerializeField] private Material unitMaterial;
        [SerializeField] private float unitScale;
        [SerializeField] private GameObject outline;

        // application quit
        bool applicationQuitFlag = false;
        private void Awake()
        {
            if (instance == null)
                instance = this;
            SetupECSPure();
        }
        private void Start()
        {
        }

        private void OnApplicationQuit()
        {
            applicationQuitFlag = true;
        }
        private void OnDestroy()
        {
            if (applicationQuitFlag == true) return;
            DestroyGrid();
        }
        public void DestroyGrid()
        {
            foreach (var entity in list_entityALL)
            {
                entityManager.DestroyEntity(entity);
            }
            foreach (var entity in list_entityXY)
            {
                entityManager.DestroyEntity(entity);
            }
            foreach (var entity in list_entityXZ)
            {
                entityManager.DestroyEntity(entity);
            }
            foreach (var entity in list_entityYZ)
            {
                entityManager.DestroyEntity(entity);
            }
        }

        private void SetupECSPure()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;
            archetype = entityManager.CreateArchetype
            (
                typeof(Translation),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(Scale),
                typeof(RenderBounds)
            );
        }
        public void ClearGridFlags()
        {
            gridFlag.All = false;
            gridFlag.XY = false;
            gridFlag.YZ = false;
            gridFlag.YZ = false;
            gridFlag.OUTLINE = false;
        }
        public void ToggleGrid(string gridType)
        {
            if (EditorUIManager.Instance.cubeDesigneInstance == null) { Debug.Log("Failed: Cube Designe does not exist"); return; }
            // restrict make all grid when cubedesign size is bigger than 20. to prevent lack.
            if ((EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber >= 20) & (gridType == "ALL")) gridType = "XYZ";

            if ((gridFlag.All == true) & (gridType != "ALL"))
            {
                DestroyGrid_All();
                gridFlag.All = false; gridFlag.XY = false; gridFlag.YZ = false; gridFlag.XZ = false;
            }

            switch (gridType)
            {
                case "XY":
                    if (gridFlag.XY == true)
                    {
                        DestroyGrid_XY();
                        gridFlag.XY = false;
                    }
                    else
                    {
                        CreateGrid_XY();
                        _gridType = e_GridType.XY;
                        gridFlag.XY = true;
                    }
                    break;
                case "XZ":
                    if (gridFlag.XZ == true)
                    {
                        DestroyGrid_XZ();
                        gridFlag.XZ = false;
                    }
                    else
                    {
                        CreateGrid_XZ();
                        _gridType = e_GridType.XZ;
                        gridFlag.XZ = true;
                    }
                    break;
                case "YZ":
                    if (gridFlag.YZ == true)
                    {
                        DestroyGrid_YZ();
                        gridFlag.YZ = false;
                    }
                    else
                    {
                        CreateGrid_YZ();
                        _gridType = e_GridType.YZ;
                        gridFlag.YZ = true;
                    }
                    break;
                case "XYZ":
                    if (gridFlag.XY == true | gridFlag.YZ == true | gridFlag.XZ == true)
                    {
                        DestroyGrid_XYZ();
                        gridFlag.XY = false; gridFlag.YZ = false; gridFlag.XZ = false;
                    }
                    else
                    {
                        CreateGrid_XYZ();
                        _gridType = e_GridType.XYZ;
                        gridFlag.XY = true; gridFlag.YZ = true; gridFlag.XZ = true;
                    }
                    break;
                case "ALL":
                    if (gridFlag.All == true | gridFlag.XY == true | gridFlag.YZ == true | gridFlag.XZ == true)
                    {
                        DestroyGrid_All();
                        gridFlag.All = false; gridFlag.XY = false; gridFlag.YZ = false; gridFlag.XZ = false;
                    }
                    else
                    {
                        CreateGrid_All();
                        _gridType = e_GridType.All;
                        gridFlag.All = true;
                    }
                    break;
                case "OUTLINE":
                    if (gridFlag.OUTLINE == true)
                    {
                        outline.SetActive(false); gridFlag.OUTLINE = false;
                    }
                    else
                    {
                        outline.SetActive(true); gridFlag.OUTLINE = true;
                    }
                    break;
                default:
                    break;
            }
            SyncGridToCurosr();
        }
        public void GridOff()
        {
            Debug.Log("grid turned off");
            if (EditorUIManager.Instance.cubeDesigneInstance == null) { Debug.Log("Failed: Cube Designe does not exist"); return; }
            DestroyGrid_All();
            ClearGridFlags();
        }
        public void GridOn()
        {
            if (EditorUIManager.Instance.cubeDesigneInstance == null) return;
            // restrict make all grid when cubedesign size is bigger than 20. to prevent lack.
            if ((EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber >= 20) & (_gridType == e_GridType.All)) _gridType = e_GridType.XYZ;

            DestroyGrid_All();

            switch (_gridType)
            {
                case e_GridType.None:
                    break;
                case e_GridType.XY:
                    CreateGrid_XY();
                    gridFlag.XY = true;
                    break;
                case e_GridType.XZ:
                    CreateGrid_XZ();
                    gridFlag.XZ = true;
                    break;
                case e_GridType.YZ:
                    CreateGrid_YZ();
                    gridFlag.YZ = true;
                    break;
                case e_GridType.XYZ:
                    CreateGrid_XYZ();
                    gridFlag.XY = true; gridFlag.YZ = true; gridFlag.XZ = true;
                    break;
                case e_GridType.All:
                    CreateGrid_All();
                    gridFlag.All = true;
                    break;
                case e_GridType.Outline:
                    outline.SetActive(true); gridFlag.OUTLINE = true;
                    break;
                default:
                    break;
            }
            SyncGridToCurosr();
        }
        public void GridOn(e_GridType gridType)
        {
            _gridType = gridType;
            GridOn();
        }
        private void DestroyGrid_All()
        {
            if (activeEntityLines == true)
            {
                foreach (var entity in list_entityALL)
                {
                    entityManager.DestroyEntity(entity);
                }
                list_entityALL.Clear();
            }

            DestroyGrid_XYZ();
        }
        private void DestroyGrid_XYZ()
        {
            DestroyGrid_XY();
            DestroyGrid_XZ();
            DestroyGrid_YZ();
        }
        private void DestroyGrid_XY()
        {
            if (activeEntityLines == true)
            {
                foreach (var entity in list_entityXY)
                {
                    entityManager.DestroyEntity(entity);
                }
                list_entityXY.Clear();
            }
            else
            {
                if (gridPlaneXY == null) return;
                Destroy(gridPlaneXY);
            }
        }
        private void DestroyGrid_XZ()
        {
            if (activeEntityLines == true)
            {
                foreach (var entity in list_entityXZ)
                {
                    entityManager.DestroyEntity(entity);
                }
                list_entityXZ.Clear();
            }
            else
            {
                if (gridPlaneXZ == null) return;
                Destroy(gridPlaneXZ);
            }
        }
        private void DestroyGrid_YZ()
        {
            if (activeEntityLines == true)
            {
                foreach (var entity in list_entityYZ)
                {
                    entityManager.DestroyEntity(entity);
                }
                list_entityYZ.Clear();
            }
            else
            {
                if (gridPlaneYZ == null) return;
                Destroy(gridPlaneYZ);
            }
        }

        private void CreateGrid_All()
        {
            int oneSideNumber = EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber;
            float oneSideLength = EditorUIManager.Instance.cubeDesigneInstance.oneSideLength;

            if (activeEntityLines == true)
            {
                ChangeMeshToGridLine(unitMesh);
                for (int i = 0; i < (oneSideNumber + 1); i++)
                {
                    for (int j = 0; j < (oneSideNumber + 1); j++)
                    {
                        Entity entity = entityManager.CreateEntity(archetype);
                        entityManager.AddComponentData(entity, new Translation
                        {
                            Value = new float3(-0.5f + (j * oneSideLength), -0.5f + (i * oneSideLength), 0f)
                        });
                        entityManager.AddSharedComponentData(entity, new RenderMesh
                        {
                            mesh = unitMesh,
                            material = unitMaterial
                        });
                        entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                        list_entityALL.Add(entity);
                        HowManyGridLinesAre++;// for debug;
                    }
                }
                for (int i = 0; i < (oneSideNumber + 1); i++)
                {
                    for (int j = 0; j < (oneSideNumber + 1); j++)
                    {
                        Entity entity = entityManager.CreateEntity(archetype);
                        entityManager.AddComponentData(entity, new Translation
                        {
                            Value = new float3(0f, -0.5f + (i * oneSideLength), -0.5f + (j * oneSideLength))
                        });
                        entityManager.AddSharedComponentData(entity, new RenderMesh
                        {
                            mesh = unitMesh,
                            material = unitMaterial
                        });
                        entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                        entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(0f, 90f, 0f) });
                        list_entityALL.Add(entity);
                        HowManyGridLinesAre++;// for debug;
                    }
                }
                for (int i = 0; i < (oneSideNumber + 1); i++)
                {
                    for (int j = 0; j < (oneSideNumber + 1); j++)
                    {
                        Entity entity = entityManager.CreateEntity(archetype);
                        entityManager.AddComponentData(entity, new Translation
                        {
                            Value = new float3(-0.5f + (i * oneSideLength), 0f, -0.5f + (j * oneSideLength))
                        });
                        entityManager.AddSharedComponentData(entity, new RenderMesh
                        {
                            mesh = unitMesh,
                            material = unitMaterial
                        });
                        entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                        entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(90f, 0f, 0f) });
                        list_entityALL.Add(entity);
                        HowManyGridLinesAre++;// for debug;
                    }
                }
                Debug.Log($"Grid created! number{HowManyGridLinesAre}");
            }
            else
            {
                CreateGrid_XY();
                CreateGrid_XZ();
                CreateGrid_YZ();
            }
        }
        private void CreateGrid_XY()
        {
            int oneSideNumber = EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber;
            if (activeEntityLines == true)
            {
                float oneSideLength = EditorUIManager.Instance.cubeDesigneInstance.oneSideLength;

                ChangeMeshToGridLine(unitMesh);
                for (int j = 0; j < (oneSideNumber + 1); j++)
                {
                    Entity entity = entityManager.CreateEntity(archetype);
                    entityManager.AddComponentData(entity, new Translation
                    {
                        Value = new float3(0f, -0.5f + (j * oneSideLength), -0.5f + (0 * oneSideLength))
                    });
                    entityManager.AddSharedComponentData(entity, new RenderMesh
                    {
                        mesh = unitMesh,
                        material = unitMaterial
                    });
                    entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                    entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(0f, 90f, 0f) });
                    list_entityXY.Add(entity);
                    HowManyGridLinesAre++;// for debug;
                }
                for (int j = 0; j < (oneSideNumber + 1); j++)
                {
                    Entity entity = entityManager.CreateEntity(archetype);
                    entityManager.AddComponentData(entity, new Translation
                    {
                        Value = new float3(-0.5f + (j * oneSideLength), 0f, -0.5f + (0 * oneSideLength))
                    });
                    entityManager.AddSharedComponentData(entity, new RenderMesh
                    {
                        mesh = unitMesh,
                        material = unitMaterial
                    });
                    entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                    entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(90f, 0f, 0f) });
                    list_entityXY.Add(entity);
                    HowManyGridLinesAre++;// for debug;
                }
            }
            else
            {
                gridPlaneMat.mainTextureScale = new Vector2(oneSideNumber / 3.0f, oneSideNumber / 3.0f);
                gridPlaneXY = Instantiate(gridPlane, cubeDesignTransform);
                gridPlaneXY.transform.Rotate(new Vector3(90f, 0f, 0f));
            }
        }
        private void CreateGrid_XZ()
        {
            int oneSideNumber = EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber;
            if (activeEntityLines == true)
            {
                float oneSideLength = EditorUIManager.Instance.cubeDesigneInstance.oneSideLength;
                ChangeMeshToGridLine(unitMesh);
                for (int j = 0; j < (oneSideNumber + 1); j++)
                {
                    Entity entity = entityManager.CreateEntity(archetype);
                    entityManager.AddComponentData(entity, new Translation
                    {
                        Value = new float3(-0.5f + (j * oneSideLength), -0.5f + (0 * oneSideLength), 0f)
                    });
                    entityManager.AddSharedComponentData(entity, new RenderMesh
                    {
                        mesh = unitMesh,
                        material = unitMaterial
                    });
                    entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                    entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(0f, 0f, 0f) });
                    list_entityXZ.Add(entity);
                    HowManyGridLinesAre++;// for debug;
                }
                for (int j = 0; j < (oneSideNumber + 1); j++)
                {
                    Entity entity = entityManager.CreateEntity(archetype);
                    entityManager.AddComponentData(entity, new Translation
                    {
                        Value = new float3(0f, -0.5f + (0 * oneSideLength), -0.5f + (j * oneSideLength))
                    });
                    entityManager.AddSharedComponentData(entity, new RenderMesh
                    {
                        mesh = unitMesh,
                        material = unitMaterial
                    });
                    entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                    entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(0f, 90f, 0f) });
                    list_entityXZ.Add(entity);
                    HowManyGridLinesAre++;// for debug;
                }
            }
            else
            {
                gridPlaneMat.mainTextureScale = new Vector2(oneSideNumber / 3.0f, oneSideNumber / 3.0f);
                gridPlaneXZ = Instantiate(gridPlane, cubeDesignTransform);
            }

        }
        private void CreateGrid_YZ()
        {
            int oneSideNumber = EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber;
            if (activeEntityLines == true)
            {
                float oneSideLength = EditorUIManager.Instance.cubeDesigneInstance.oneSideLength;
                ChangeMeshToGridLine(unitMesh);
                for (int j = 0; j < (oneSideNumber + 1); j++)
                {
                    Entity entity = entityManager.CreateEntity(archetype);
                    entityManager.AddComponentData(entity, new Translation
                    {
                        Value = new float3(-0.5f + (0 * oneSideLength), -0.5f + (j * oneSideLength), 0f)
                    });
                    entityManager.AddSharedComponentData(entity, new RenderMesh
                    {
                        mesh = unitMesh,
                        material = unitMaterial
                    });
                    entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                    entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(0f, 0f, 0f) });
                    list_entityYZ.Add(entity);
                    HowManyGridLinesAre++;// for debug;
                }
                for (int j = 0; j < (oneSideNumber + 1); j++)
                {
                    Entity entity = entityManager.CreateEntity(archetype);
                    entityManager.AddComponentData(entity, new Translation
                    {
                        Value = new float3(-0.5f + (0 * oneSideLength), 0f, -0.5f + (j * oneSideLength))
                    });
                    entityManager.AddSharedComponentData(entity, new RenderMesh
                    {
                        mesh = unitMesh,
                        material = unitMaterial
                    });
                    entityManager.AddComponentData(entity, new Scale { Value = unitScale });
                    entityManager.AddComponentData(entity, new Rotation { Value = Quaternion.Euler(90f, 0f, 0f) });
                    list_entityYZ.Add(entity);
                    HowManyGridLinesAre++;// for debug;
                }
            }
            else
            {
                gridPlaneMat.mainTextureScale = new Vector2(oneSideNumber / 3.0f, oneSideNumber / 3.0f);
                gridPlaneYZ = Instantiate(gridPlane, cubeDesignTransform);
                gridPlaneYZ.transform.Rotate(new Vector3(0f, 0f, 90f));
            }
        }
        private void CreateGrid_XYZ()
        {
            CreateGrid_XY();
            CreateGrid_XZ();
            CreateGrid_YZ();
        }
        private void ChangeMeshToGridLine(Mesh mesh)
        {
            float osl = 0.5f;// mesh scale factor
                             //Vertices//
            Vector3[] vertices = new Vector3[]
            {
		//front face//
		new Vector3(-gridThickness, gridThickness, -osl),//left top front, 0
        new Vector3(gridThickness, gridThickness, -osl),//right top front, 1
		new Vector3(-gridThickness,-gridThickness,-osl),//left bottom front, 2
		new Vector3(gridThickness,-gridThickness,-osl),//right bottom front, 3

		//back face//
		new Vector3(gridThickness, gridThickness, osl),//right top back, 4
		new Vector3(-gridThickness, gridThickness, osl),//left top back, 5
		new Vector3(gridThickness,-gridThickness,osl),//right bottom back, 6
		new Vector3(-gridThickness,-gridThickness,osl),//left bottom back, 7

		//left face//
		new Vector3(-gridThickness, gridThickness, osl),//left top back, 8
		new Vector3(-gridThickness, gridThickness, -osl),//left top front, 9
		new Vector3(-gridThickness,-gridThickness,osl),//left bottom back, 10
		new Vector3(-gridThickness,-gridThickness,-osl),//left bottom front, 11

		//right face//
		new Vector3(gridThickness, gridThickness, -osl),//right top front, 12
		new Vector3(gridThickness, gridThickness, osl),//right top back, 13
		new Vector3(gridThickness,-gridThickness,-osl),//right bottom front, 14
		new Vector3(gridThickness,-gridThickness,osl),//right bottom back, 15

		//top face//
		new Vector3(-gridThickness, gridThickness, osl),//left top back, 16
		new Vector3(gridThickness, gridThickness, osl),//right top back, 17
		new Vector3(-gridThickness,gridThickness,-osl),//left top front, 18
		new Vector3(gridThickness,gridThickness,-osl),//right top front, 19

		//bottom face//
		new Vector3(-gridThickness,-gridThickness,-osl),//left bottom front, 20
		new Vector3(gridThickness,-gridThickness,-osl),//right bottom front, 21
		new Vector3(-gridThickness,-gridThickness,osl),//left bottom back, 22
		new Vector3(gridThickness,-gridThickness,osl)//right bottom back, 23

            };

            //Triangles// 3 points, clockwise determines which side is visible
            int[] triangles = new int[]
            {
		//front face//
		0,2,3,//first triangle
		3,1,0,//second triangle

		//back face//
		4,6,7,//first triangle
		7,5,4,//second triangle

		//left face//
		8,10,11,//first triangle
		11,9,8,//second triangle

		//right face//
		12,14,15,//first triangle
		15,13,12,//second triangle

		//top face//
		16,18,19,//first triangle
		19,17,16,//second triangle

		//bottom face//
		20,22,23,//first triangle
		23,21,20//second triangle
            };

            //UVs//
            Vector2[] uvs = new Vector2[]
            {
		//front face// 0,0 is bottom left, 1,1 is top right//
		new Vector2(0,osl),
        new Vector2(0,0),
        new Vector2(osl,osl),
        new Vector2(osl,0),

        new Vector2(0,osl),
        new Vector2(0,0),
        new Vector2(osl,osl),
        new Vector2(osl,0),

        new Vector2(0,osl),
        new Vector2(0,0),
        new Vector2(osl,osl),
        new Vector2(osl,0),

        new Vector2(0,osl),
        new Vector2(0,0),
        new Vector2(osl,osl),
        new Vector2(osl,0),

        new Vector2(0,osl),
        new Vector2(0,0),
        new Vector2(osl,osl),
        new Vector2(osl,0),

        new Vector2(0,osl),
        new Vector2(0,0),
        new Vector2(osl,osl),
        new Vector2(osl,0)


            };

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.Optimize();
            mesh.RecalculateNormals();
        }

        public void SyncGridToCurosr()
        {
            int3 CursorIdx = EditorUIManager.Instance.cubeDesigneInstance.GetCursorIdx();
            if (CursorIdx.x < 0 | CursorIdx.y < 0 | CursorIdx.z < 0) return; // no cursor.
            int oneSideNumber = EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber;
            float oneSideLength = EditorUIManager.Instance.cubeDesigneInstance.oneSideLength;

            if (activeEntityLines == true)
            {
                Translation translation;

                // xy sync
                foreach (var entity in list_entityXY)
                {
                    translation = entityManager.GetComponentData<Translation>(entity);
                    translation.Value.z = CursorIdx.z * oneSideLength - 0.5f;
                    entityManager.SetComponentData<Translation>(entity, translation);
                }

                // xz sync
                foreach (var entity in list_entityXZ)
                {
                    translation = entityManager.GetComponentData<Translation>(entity);
                    translation.Value.y = CursorIdx.y * oneSideLength - 0.5f;
                    entityManager.SetComponentData<Translation>(entity, translation);
                }

                // yz sync
                foreach (var entity in list_entityYZ)
                {
                    translation = entityManager.GetComponentData<Translation>(entity);
                    translation.Value.x = CursorIdx.x * oneSideLength - 0.5f;
                    entityManager.SetComponentData<Translation>(entity, translation);
                }

            }
            else
            {
                Vector3 tmpPos;
                if (gridPlaneXY != null)
                {
                    tmpPos = new Vector3(0f, 0f, CursorIdx.z * oneSideLength - 0.5f);
                    gridPlaneXY.transform.position = tmpPos;
                }
                if (gridPlaneXZ != null)
                {
                    tmpPos = new Vector3(0f, CursorIdx.y * oneSideLength - 0.5f, 0f);
                    gridPlaneXZ.transform.position = tmpPos;
                }
                if (gridPlaneYZ != null)
                {
                    tmpPos = new Vector3(CursorIdx.x * oneSideLength - 0.5f, 0f, 0f);
                    gridPlaneYZ.transform.position = tmpPos;
                }
            }
        }
    }
}
