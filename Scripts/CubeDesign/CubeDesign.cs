using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Jobs;
using Unity.Burst;
using System.Threading.Tasks;
using Unity.Physics.Authoring;

namespace DOTS
{
    public class CubeDesign : MonoBehaviour
    {
        [HideInInspector] public int resolution;
        [HideInInspector] public float oneSideLength;
        [HideInInspector] public int oneSideNumber;

        [Header("Cursor")]
        [SerializeField] Transform zero;
        [SerializeField] GameObject cursorPrefab;
        [SerializeField] Mesh defaultMesh;
        private GameObject cursorObj;
        private Entity cursorBrickEntity;
        private BrickInfo cursorInfoDefault;
        [HideInInspector] public bool isCursorExists;

        private Translation cursorTranslation;
        private BrickInfo cursorBrickInfo;
        private Mesh cursorMesh;
        private Material cursorMaterial;

        [Header("Unit")]
        [SerializeField] string unitMaterialCategory;
        [SerializeField] Material unitMaterial;
        [SerializeField] private float unitScale;

        [HideInInspector] public bool _isSaved;
        [HideInInspector] public int totalBrickNumber;

        private EntityManager entityManager;
        private Entity[] entities;
        private World defaultWorld;
        private EntityArchetype brickEntityArchtype;
        private EntityArchetype brickCursorEntityArchtype;

        private float3 cursorRotationValue = new float3(0f, 0f, 0f);

        // application quit
        bool applicationQuitFlag = false;

        // info
        [HideInInspector] public int maxIdx;

        // Constructing brick
        [HideInInspector] public bool autoPut = false;
        [HideInInspector] public bool autoDelete = false;

        private void Awake()
        {
            SetupECSPure();
            SetSavedFlag(true);
        }
        //---------init-------------
        public CMDState initState = CMDState.BUSY;
        Coroutine coroutine;
        //--------------------------

        private void Start()
        {
            StartCoroutine(InitSetting());
            Physics.autoSyncTransforms = true; // transform calc optimzation
            Physics.reuseCollisionCallbacks = true; // collision call back  optimization
            CubeDesignRayCast.instance.SetCubeDesign(this);
        }
        private void OnApplicationQuit()
        {
            applicationQuitFlag = true;
        }
        private void OnDestroy()
        {
            if (applicationQuitFlag == true) return;
            DestroyAllEntities();
        }
        IEnumerator InitSetting()
        {
            initState = CMDState.BUSY;
            coroutine = StartCoroutine(GetUnitMaterialDataFromDataManager());
            yield return new WaitUntil(() => coroutine == null);
            SetCursorDefaultSetting();
            CreateCursor();
            initState = CMDState.IDLE;
            //Debug.Log("CubeDesign : InitSetting() completed !!");
        }
        IEnumerator GetUnitMaterialDataFromDataManager()
        {
            yield return new WaitUntil(() => DataManager.instance.initState == CMDState.IDLE);
            unitMaterialCategory = DataManager.instance.unitMaterialCategory;
            unitMaterial = DataManager.instance.unitMaterial;
            StopCoroutine(coroutine);
            coroutine = null;
        }
        private void SetCursorDefaultSetting()
        {
            cursorInfoDefault = new BrickInfo();
            cursorInfoDefault._id = CalcPositionToIndex(new float3(zero.position.x, zero.position.y, zero.position.z));
            cursorInfoDefault._isExist = false;
            cursorInfoDefault._meshName = "";
            cursorInfoDefault._matCategory = unitMaterialCategory;
            cursorInfoDefault._matName = unitMaterial.name;
            cursorInfoDefault._rotationValue = new float3(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z);
        }
        public void DestroyAllEntities()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != Entity.Null)
                {
                    entityManager.DestroyEntity(entities[i]);
                    entities[i] = Entity.Null;
                }
            }
        }
        //==========================================================================
        // Cube Designer specifications....
        //==========================================================================
        public void SelectPixel(int inputResolution)
        {
            if (inputResolution < 1)
            {
                Debug.LogError("Invalid resolution");
            }
            resolution = inputResolution;
            oneSideNumber = resolution;
            oneSideLength = 1.0f / oneSideNumber;
            unitScale /= oneSideNumber;
            maxIdx = oneSideNumber * oneSideNumber * oneSideNumber - 1;

            entities = new Entity[maxIdx + 1];
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i] = Entity.Null;
            }
            // zeroing
            zero.localPosition = new Vector3(-0.5f + oneSideLength / 2, -0.5f + oneSideLength / 2, -0.5f + oneSideLength / 2);
        }

        //===========================================================================
        // cursor functions
        //===========================================================================
        // when user select brick,  Change cursor same as selected brick.
        public void CreateCursor()
        {
            Translation cursorBrickEntityTranslation;
            RenderMesh cursorBrickEntityRenderMesh;
            BrickInfo cursorBrickInfo = cursorInfoDefault;
            cursorBrickEntityTranslation = new Translation
            {
                Value = new float3(zero.position.x,
                                   zero.position.y,
                                   zero.position.z)
            };
            cursorBrickEntityRenderMesh.material = unitMaterial;

            cursorBrickEntity = entityManager.CreateEntity(brickCursorEntityArchtype);

            entityManager.AddComponentData(cursorBrickEntity, new BrickInfo
            {
                _id = CalcPositionToIndex(cursorBrickEntityTranslation.Value),
                _isExist = true,
                _meshName = cursorInfoDefault._meshName,
                _matCategory = cursorBrickInfo._matCategory,
                _matName = cursorBrickInfo._matName,
                _rotationValue = cursorRotationValue,
                _scaleFactor = 1.0f
            });
            entityManager.AddComponentData(cursorBrickEntity, new Translation
            {
                Value = cursorBrickEntityTranslation.Value
            });
            entityManager.AddComponentData(cursorBrickEntity, new Rotation
            {
                Value = Quaternion.identity
            });
            entityManager.AddSharedComponentData(cursorBrickEntity, new RenderMesh
            {
                mesh = defaultMesh,
                material = cursorBrickEntityRenderMesh.material,
            });

            entityManager.AddComponentData(cursorBrickEntity, new Scale { Value = unitScale * 0.005f });
            entityManager.AddComponentData(cursorBrickEntity, new LocalToWorld { });

            cursorTranslation = cursorBrickEntityTranslation;
            cursorMesh = defaultMesh;
            cursorMaterial = cursorBrickEntityRenderMesh.material;

            cursorObj = MonoBehaviour.Instantiate(cursorPrefab, transform);
            cursorObj.transform.position = new Vector3(cursorBrickEntityTranslation.Value.x,
                                                       cursorBrickEntityTranslation.Value.y,
                                                       cursorBrickEntityTranslation.Value.z);
            cursorObj.transform.localScale = new Vector3(unitScale * 1.005f, unitScale * 1.005f, unitScale * 1.005f);
            //Debug.Log("created cursor");
            // test
            MoveCursor_Yplus();
            MoveCursor_Yminus();
            SelectBrick(defaultMesh);

            isCursorExists = true;
        }
        public void DestroyCursor()
        {
            entityManager.DestroyEntity(cursorBrickEntity);
            MonoBehaviour.Destroy(cursorObj);
            isCursorExists = false;
        }
        public void SelectBrick(Mesh brickMesh)
        {
            Translation cursorBrickEntityTranslation;
            RenderMesh cursorBrickEntityRenderMesh;
            BrickInfo cursorBrickInfo = cursorInfoDefault;

            if (cursorBrickEntity != Entity.Null)
            {
                cursorBrickEntityTranslation = entityManager.GetComponentData<Translation>(cursorBrickEntity);
                cursorBrickEntityRenderMesh = entityManager.GetSharedComponentData<RenderMesh>(cursorBrickEntity);
                cursorBrickInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
                entityManager.DestroyEntity(cursorBrickEntity);
            }
            else
            {
                cursorBrickEntityTranslation = new Translation
                {
                    Value = new float3(zero.position.x,
                                       zero.position.y,
                                       zero.position.z)
                };
                cursorBrickEntityRenderMesh.material = unitMaterial;
            }
            cursorBrickEntity = entityManager.CreateEntity(brickCursorEntityArchtype);

            entityManager.AddComponentData(cursorBrickEntity, new BrickInfo
            {
                _id = CalcPositionToIndex(cursorBrickEntityTranslation.Value),
                _isExist = true,
                _meshName = brickMesh.name,
                _matCategory = cursorBrickInfo._matCategory,
                _matName = cursorBrickInfo._matName,
                _rotationValue = cursorRotationValue,
                _scaleFactor = 1.0f
            });
            entityManager.AddComponentData(cursorBrickEntity, new Translation
            {
                Value = cursorBrickEntityTranslation.Value
            });
            entityManager.AddComponentData(cursorBrickEntity, new Rotation
            {
                Value = Quaternion.identity
            });
            entityManager.AddSharedComponentData(cursorBrickEntity, new RenderMesh
            {
                mesh = brickMesh,
                material = cursorBrickEntityRenderMesh.material,
            });
            cursorMesh = brickMesh;
            entityManager.AddComponentData(cursorBrickEntity, new Scale { Value = unitScale * 0.005f });
            entityManager.AddComponentData(cursorBrickEntity, new LocalToWorld { });

            cursorMesh = brickMesh;
            //Debug.Log($"Mesh {brickMesh.name} is Selected");
        }

        public void ChangeCursorMaterial(Material material, string Category)
        {
            // change material
            if (cursorBrickEntity == Entity.Null) return;
            RenderMesh tmpRenderMesh = entityManager.GetSharedComponentData<RenderMesh>(cursorBrickEntity);
            tmpRenderMesh.material = material;
            entityManager.SetSharedComponentData<RenderMesh>(cursorBrickEntity, tmpRenderMesh);
            cursorMaterial = material;
            // save data
            BrickInfo brickInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
            brickInfo._matCategory = Category;
            brickInfo._matName = tmpRenderMesh.material.name;
            entityManager.SetComponentData<BrickInfo>(cursorBrickEntity, brickInfo);
            //Debug.Log($"Material has changed as [{brickInfo._matCategory}] of [{brickInfo._matName}]");
        }

        public void ResetCursorMaterial()
        {
            // reset material
            if (cursorBrickEntity == Entity.Null) return;
            RenderMesh tmpRenderMesh = entityManager.GetSharedComponentData<RenderMesh>(cursorBrickEntity);
            tmpRenderMesh.material = unitMaterial;
            entityManager.SetSharedComponentData<RenderMesh>(cursorBrickEntity, tmpRenderMesh);
            cursorMaterial = unitMaterial;
            // save data
            BrickInfo brickInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
            brickInfo._matCategory = unitMaterialCategory;
            brickInfo._matName = unitMaterial.name;
            entityManager.SetComponentData<BrickInfo>(cursorBrickEntity, brickInfo);          
        }

        public void RotateCusor_X()
        {
            RotateBrick(90f, 0f, 0f);
        }
        public void RotateCusor_Y()
        {
            RotateBrick(0f, 90f, 0f);
        }
        public void RotateCusor_Z()
        {
            RotateBrick(0f, 0f, 90f);
        }
        public void RotateCursor(float rotateValue_X, float rotateValue_Y, float rotateValue_Z)
        {
            cursorRotationValue.x += rotateValue_X;
            if (cursorRotationValue.x >= 360f) cursorRotationValue.x = 0f;
            cursorRotationValue.y += rotateValue_Y;
            if (cursorRotationValue.y >= 360f) cursorRotationValue.y = 0f;
            cursorRotationValue.z += rotateValue_Z;
            if (cursorRotationValue.z >= 360f) cursorRotationValue.z = 0f;

            BrickInfo brickInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
            brickInfo._rotationValue = cursorRotationValue;
            entityManager.SetComponentData<BrickInfo>(cursorBrickEntity, brickInfo);
            Rotation rotation = entityManager.GetComponentData<Rotation>(cursorBrickEntity);
            rotation.Value = Quaternion.Euler(cursorRotationValue.x, cursorRotationValue.y, cursorRotationValue.z); //math.mul(rotation.Value, quaternion.RotateY(math.degrees(90)));
            entityManager.SetComponentData<Rotation>(cursorBrickEntity, rotation);
        }
        public void RotateBrick(float rotateValue_X, float rotateValue_Y, float rotateValue_Z)
        {
            int cursorID = GetCursorID();
            if (IsBrickExistOnIndex(cursorID) == false) return;
            BrickInfo brickInfo = entityManager.GetComponentData<BrickInfo>(entities[cursorID]);
            float3 brickRotationValue = brickInfo._rotationValue;

            brickRotationValue.x += rotateValue_X;
            if (brickRotationValue.x >= 360f) brickRotationValue.x = 0f;
            brickRotationValue.y += rotateValue_Y;
            if (brickRotationValue.y >= 360f) brickRotationValue.y = 0f;
            brickRotationValue.z += rotateValue_Z;
            if (brickRotationValue.z >= 360f) brickRotationValue.z = 0f;

            brickInfo._rotationValue = brickRotationValue;
            entityManager.SetComponentData<BrickInfo>(entities[cursorID], brickInfo);
            Rotation rotation = entityManager.GetComponentData<Rotation>(entities[cursorID]);
            rotation.Value = Quaternion.Euler(brickRotationValue.x, brickRotationValue.y, brickRotationValue.z); //math.mul(rotation.Value, quaternion.RotateY(math.degrees(90)));
            entityManager.SetComponentData<Rotation>(entities[cursorID], rotation);

            SetCursorRotation(brickRotationValue.x, brickRotationValue.y, brickRotationValue.z);
        }
        public void ScaleUpBrick()
        {
            int cursorID = GetCursorID();
            if (IsBrickExistOnIndex(cursorID) == false) return;
            BrickInfo brickInfo = entityManager.GetComponentData<BrickInfo>(entities[cursorID]);

            if (brickInfo._scaleFactor >= 2.0f)
            {
                brickInfo._scaleFactor = 0.5f;
            }
            else
            {
                brickInfo._scaleFactor += 0.5f;
            }
            entityManager.SetComponentData<BrickInfo>(entities[cursorID], brickInfo);
            entityManager.SetComponentData(entities[cursorID], new Scale { Value = unitScale * brickInfo._scaleFactor });
        }

        public void SetCursorRotation(float rotationValue_X, float rotationValue_Y, float rotationValue_Z)
        {
            cursorRotationValue = new float3(rotationValue_X, rotationValue_Y, rotationValue_Z);
            BrickInfo brickInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
            brickInfo._rotationValue = cursorRotationValue;
            entityManager.SetComponentData<BrickInfo>(cursorBrickEntity, brickInfo);
            Rotation rotation = entityManager.GetComponentData<Rotation>(cursorBrickEntity);
            rotation.Value = Quaternion.Euler(cursorRotationValue.x, cursorRotationValue.y, cursorRotationValue.z); //math.mul(rotation.Value, quaternion.RotateY(math.degrees(90)));
            entityManager.SetComponentData<Rotation>(cursorBrickEntity, rotation);

        }
        public void MoveCursor_Xplus()
        {
            int moveUnitID = 1;

            if (cursorBrickEntity == Entity.Null) return;
            // check position idx validity
            int cursorID = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;
            if ((cursorID / moveUnitID % oneSideNumber) >= (oneSideNumber - 1))
            {
                //Debug.Log("Failed to Move Cursor : it's x+ limit");
                return;
            }
            int targetID = cursorID + moveUnitID;
            MoveCursorTo(targetID);
            if (autoPut == true) PutBrick(true);
            if (autoDelete == true) DeleteBrick(true);
        }
        public void MoveCursor_Xminus()
        {
            int moveUnitID = 1;

            if (cursorBrickEntity == Entity.Null) return;
            // check position idx validity
            int cursorID = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;

            if ((cursorID / moveUnitID % oneSideNumber) <= 0)
            {
                //Debug.Log("Failed to Move Cursor : it's x- limit");
                return;
            }
            int targetID = cursorID - moveUnitID;
            MoveCursorTo(targetID);
            if (autoPut == true) PutBrick(true);
            if (autoDelete == true) DeleteBrick(true);
        }
        public void MoveCursor_Yplus()
        {
            int moveUnitID = oneSideNumber;

            if (cursorBrickEntity == Entity.Null) return;
            // check position idx validity
            int cursorID = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;

            if ((cursorID / moveUnitID % oneSideNumber) >= (oneSideNumber - 1))
            {
                //Debug.Log("Failed to Move Cursor : it's y+ limit");
                return;
            }
            int targetID = cursorID + moveUnitID;
            MoveCursorTo(targetID);
            if (autoPut == true) PutBrick(true);
            if (autoDelete == true) DeleteBrick(true);
        }
        public void MoveCursor_Yminus()
        {
            int moveUnitID = oneSideNumber;

            if (cursorBrickEntity == Entity.Null) return;
            // check position idx validity
            int cursorID = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;
            if ((cursorID / moveUnitID % oneSideNumber) <= 0)
            {
                Debug.Log("Failed to Move Cursor : it's y- limit");
                return;
            }
            int targetID = cursorID - moveUnitID;
            MoveCursorTo(targetID);
            if (autoPut == true) PutBrick(true);
            if (autoDelete == true) DeleteBrick(true);
        }
        public void MoveCursor_Zplus()
        {
            int moveUnitID = oneSideNumber * oneSideNumber;

            if (cursorBrickEntity == Entity.Null) return;
            // check position idx validity
            int cursorID = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;

            if ((cursorID / moveUnitID % oneSideNumber) >= oneSideNumber - 1)
            {
                //Debug.Log("Failed to Move Cursor : it's Z+ limit");
                return;
            }
            int targetID = cursorID + moveUnitID;
            MoveCursorTo(targetID);
            if (autoPut == true) PutBrick(true);
            if (autoDelete == true) DeleteBrick(true);
        }
        public void MoveCursor_Zminus()
        {
            int moveUnitID = oneSideNumber * oneSideNumber;

            if (cursorBrickEntity == Entity.Null) return;
            // check position idx validity
            int cursorID = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;

            if (cursorID / moveUnitID % oneSideNumber <= 0)
            {
                //Debug.Log("Failed to Move Cursor : it's Z- limit");
                return;
            }
            int targetID = cursorID - moveUnitID;
            MoveCursorTo(targetID);

            if (autoPut == true) PutBrick(true);
            if (autoDelete == true) DeleteBrick(true);
        }
        public void MoveCursorTo(int targetID)
        {
            BrickInfo cursorInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
            Translation tmpTranslation = entityManager.GetComponentData<Translation>(cursorBrickEntity);

            cursorInfo._id = targetID;
            entityManager.SetComponentData(cursorBrickEntity, cursorInfo);
            tmpTranslation.Value = CalcIndexToPosition(cursorInfo._id);
            entityManager.SetComponentData(cursorBrickEntity, tmpTranslation);
            cursorTranslation = tmpTranslation;

            cursorObj.transform.position = new Vector3(tmpTranslation.Value.x,
                                                       tmpTranslation.Value.y,
                                                       tmpTranslation.Value.z);

            
            // sync grid
            if (CubeDesignGrid.instance == null) return;
            CubeDesignGrid.instance.SyncGridToCurosr();

            CameraHandler.instance.Refresh();// to do -> seperate camera for cursor and main..
        }
        public void MoveCursorTo(Entity targetEntity)
        {
            BrickInfo targetInfo = entityManager.GetComponentData<BrickInfo>(targetEntity);
            MoveCursorTo(targetInfo._id);
        }
        public void EnableCursor()
        {
            if (cursorObj == null) return;
            cursorObj.SetActive(true);
            if (cursorBrickEntity == Entity.Null) return;
            entityManager.RemoveComponent<Disabled>(cursorBrickEntity);
        }

        public void DisableCursor()
        {
            if (cursorObj == null) return;
            cursorObj.SetActive(false);            
            if (cursorBrickEntity == Entity.Null) return;
            entityManager.AddComponent<Disabled>(cursorBrickEntity);
        }


        public int GetCursorID()
        {
            if (entityManager.Exists(cursorBrickEntity) == false) return -1;

            int id = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;
            return id;
        }

        public int3 GetCursorIdx()
        {
            int id = GetCursorID();
            int x_idx = id % oneSideNumber;
            int y_idx = id / oneSideNumber % oneSideNumber;
            int z_idx = id / oneSideNumber / oneSideNumber % oneSideNumber;

            return new int3(x_idx, y_idx, z_idx);
        }
        public Translation GetCursorTranslation()
        {
            return cursorTranslation; //entityManager.GetComponentData<Translation>(cursorBrickEntity);
        }

        public BrickInfo GetCursorInfo()
        {
            return cursorBrickInfo;// entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
        }
        public Mesh GetCursorMesh()
        {   
            //RenderMesh cursorBrickEntityRenderMesh = entityManager.GetSharedComponentData<RenderMesh>(cursorBrickEntity);
            return cursorMesh;// cursorBrickEntityRenderMesh.mesh;
        }
        public Material GetCursorMaterial()
        {
            //RenderMesh cursorBrickEntityRenderMesh = entityManager.GetSharedComponentData<RenderMesh>(cursorBrickEntity);
            return cursorMaterial;//cursorBrickEntityRenderMesh.material;
        }
        public bool isThisIDLimitOf(int id, string direction)
        {
            bool isLimit = false;
            switch (direction)
            {
                case "x+":
                    if ((id % oneSideNumber) >= oneSideNumber - 1) isLimit = true;
                    break;
                case "x-":
                    if ((id % oneSideNumber) <= 0) isLimit = true;
                    break;
                case "y+":
                    if ((id / oneSideNumber % oneSideNumber) >= oneSideNumber - 1) isLimit = true;
                    break;
                case "y-":
                    if ((id / oneSideNumber % oneSideNumber) <= 0) isLimit = true;
                    break;
                case "z+":
                    if ((id / oneSideNumber / oneSideNumber % oneSideNumber) >= oneSideNumber - 1) isLimit = true;
                    break;
                case "z-":
                    if ((id / oneSideNumber / oneSideNumber % oneSideNumber) <= 0) isLimit = true;
                    break;
                default:
                    break;
            }
            return isLimit;
        }
        //============================================================================
        // Brick functions 
        //============================================================================

        public void OnPutClicked()
        {
            if (cursorBrickEntity == Entity.Null) return;
            PutBrick(true);
        }
        public void OnDeleteClicked()
        {
            int selectedNum = BrickSelectionManager.instance.selectedNum;
            if (selectedNum == 0)
            {
                DeleteBrick(true);
            }
            else
            {
                for (int i = 0; i <= EditorUIManager.Instance.cubeDesigneInstance.maxIdx; i++)
                {
                    DeleteBrick(i);
                }
            }
        }

        public void PutBrick(bool playSound)
        {
            // get cursor info
            BrickInfo cursorBrickInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
            Translation cursorBrickEntityTranslation = entityManager.GetComponentData<Translation>(cursorBrickEntity);
            RenderMesh cursorBrickEntityRenderMesh = entityManager.GetSharedComponentData<RenderMesh>(cursorBrickEntity);
            //Material mat;
            //if (PalletManager.Instance != null) { mat = PalletManager.Instance.palletSelectButtons[cursorMatNum].GetMaterial(); }
            //else if (PalletManagerForPro.Instance != null) { mat = PalletManagerForPro.Instance.palletSelectButtons[cursorMatNum].GetMaterial(); }
            //else return; // Todo -> Pallet Manager For Kids & For Pro need base class!!

            Rotation cursorBrickEntityRotation = entityManager.GetComponentData<Rotation>(cursorBrickEntity);

            // check brick is already exist
            if (entities[cursorBrickInfo._id] != Entity.Null)
            {
                DeleteBrick(false);
            }
            // create new entity
            Entity entity = entityManager.CreateEntity(brickEntityArchtype);

            // apply cursor info to newly created entity.
            entityManager.AddComponentData(entity, new BrickInfo
            {
                _id = cursorBrickInfo._id,
                _isExist = true,
                _meshName = cursorBrickInfo._meshName,
                _matCategory = cursorBrickInfo._matCategory,
                _matName = cursorBrickInfo._matName,
                _rotationValue = cursorBrickInfo._rotationValue,
                _scaleFactor = cursorBrickInfo._scaleFactor,
            });
            entityManager.AddComponentData(entity, new Translation
            {
                Value = cursorBrickEntityTranslation.Value
            });
            entityManager.AddComponentData(entity, new Rotation
            {
                Value = cursorBrickEntityRotation.Value
            });
            entityManager.AddSharedComponentData(entity, new RenderMesh
            {
                mesh = cursorBrickEntityRenderMesh.mesh,
                material = cursorBrickEntityRenderMesh.material, 
            });

            entityManager.AddComponentData(entity, new Scale { Value = unitScale });
            entityManager.AddComponentData(entity, new LocalToWorld { });

            Unity.Physics.BoxGeometry boxGeometry = new Unity.Physics.BoxGeometry()
            {
                Orientation = transform.rotation,
                Size = oneSideLength
            };
            /*NativeArray<float3> vertices = new NativeArray<float3>(cursorBrickEntityRenderMesh.mesh.vertices.Length, Allocator.Temp);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = cursorBrickEntityRenderMesh.mesh.vertices[i];
            }
            NativeArray<int3> triangles = new NativeArray<int3>(cursorBrickEntityRenderMesh.mesh.triangles.Length, Allocator.Temp);
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = cursorBrickEntityRenderMesh.mesh.triangles[i];
            }
            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.MeshCollider.Create(vertices, triangles);*/
            
            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.BoxCollider.Create(boxGeometry);
            
            Unity.Physics.PhysicsCollider physicsCollider = new Unity.Physics.PhysicsCollider { Value = collider };
            
            if (collider.IsCreated)
            {
                entityManager.AddComponentData(entity, physicsCollider);
            }

            /*entityManager.AddComponentData(entity, new PhysicsDebugDisplayData
            {  
                DrawColliders = 1,
                DrawColliderEdges = 1,
                DrawColliderAabbs = 1,
                DrawBroadphase = 1,
                DrawMassProperties = 1,
                DrawContacts = 1,
                DrawCollisionEvents = 1,
                DrawTriggerEvents = 1,
                DrawJoints = 1,
            });*/

            //meshPointsArray.Dispose();
            entities[cursorBrickInfo._id] = entity;
            totalBrickNumber++;

            if (playSound == true) PlayPutSound();
            _isSaved = false;
        }
        public void PutBrick(bool playSound, float scaleFactor)
        {
            // get cursor info
            BrickInfo cursorBrickInfo = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity);
            Translation cursorBrickEntityTranslation = entityManager.GetComponentData<Translation>(cursorBrickEntity);
            RenderMesh cursorBrickEntityRenderMesh = entityManager.GetSharedComponentData<RenderMesh>(cursorBrickEntity);

            Rotation cursorBrickEntityRotation = entityManager.GetComponentData<Rotation>(cursorBrickEntity);

            // check brick is already exist
            if (entityManager.Exists(entities[cursorBrickInfo._id]) == true)
            {
                DeleteBrick(false);
            }
            // create new entity
            Entity entity = entityManager.CreateEntity(brickEntityArchtype); ;

            // apply cursor info to newly created entity.
            entityManager.AddComponentData(entity, new BrickInfo
            {
                _id = cursorBrickInfo._id,
                _isExist = true,
                _meshName = cursorBrickInfo._meshName,
                _matCategory = cursorBrickInfo._matCategory,
                _matName = cursorBrickInfo._matName,
                _rotationValue = cursorBrickInfo._rotationValue,
                _scaleFactor = scaleFactor,
            });
            entityManager.AddComponentData(entity, new Translation
            {
                Value = cursorBrickEntityTranslation.Value
            });
            entityManager.AddComponentData(entity, new Rotation
            {
                Value = cursorBrickEntityRotation.Value
            });
            entityManager.AddSharedComponentData(entity, new RenderMesh
            {
                mesh = cursorBrickEntityRenderMesh.mesh,
                material = cursorBrickEntityRenderMesh.material,
            });

            //NativeArray<float3> meshPointsArray = new NativeArray<float3>(cursorBrickEntityRenderMesh.mesh.vertices.Length, Allocator.Persistent);
            //BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.ConvexCollider.Create(meshPointsArray, default, Unity.Physics.CollisionFilter.Default);
            Unity.Physics.BoxGeometry boxGeometry = new Unity.Physics.BoxGeometry()
            {
                Orientation = transform.rotation,
                Size = oneSideLength
            };
            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.BoxCollider.Create(boxGeometry);
            Unity.Physics.PhysicsCollider physicsCollider = new Unity.Physics.PhysicsCollider { Value = collider };

            if (collider.IsCreated)
            {
                entityManager.AddComponentData(entity, physicsCollider);
            }

            //meshPointsArray.Dispose();

            entityManager.AddComponentData(entity, new Scale { Value = unitScale * scaleFactor });
            entityManager.AddComponentData(entity, new LocalToWorld { });

            entities[cursorBrickInfo._id] = entity;
            totalBrickNumber++;

            if (playSound == true) PlayPutSound();
            _isSaved = false;
        }
        public void PutBrick(int positionIndex, Mesh mesh, string matCategory, Material mat, float3 rotationValue, float scaleFactor)
        {
            // set cursor
            SelectBrick(mesh);
            SetCursorRotation(rotationValue.x, rotationValue.y, rotationValue.z);
            MoveCursorTo(positionIndex);
            ChangeCursorMaterial(mat, matCategory);
            // put brick
            PutBrick(false, scaleFactor);
        }

        /*struct CreateColliderJob : IJob
        {
            public int Id;
            public Entity e;
            public NativeArray<float3> points;
            public void Execute()
            {
                EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
                
                BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.ConvexCollider.Create(points, default, Unity.Physics.CollisionFilter.Default);
                Unity.Physics.PhysicsCollider physicsCollider = new Unity.Physics.PhysicsCollider { Value = collider };
                if (collider.IsCreated)
                {
                    Debug.Log($"physics collider validity {physicsCollider.IsValid}");
                    commandBuffer.SetComponent(e, physicsCollider);
                }
                commandBuffer.Dispose();
            }
        }*/
        
        static public BlobAssetReference<Unity.Physics.Collider> CreateCollider(UnityEngine.Mesh mesh)
        {
            NativeArray<float3> points = new NativeArray<float3>(mesh.vertices.Length, Allocator.TempJob);
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                points[i] = mesh.vertices[i];
            }
            BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.ConvexCollider.Create(points, default, Unity.Physics.CollisionFilter.Default);
            points.Dispose();
            return collider;
        }

        public void CopyBrick(int referenceID, int targetID)
        {

        }

        public bool IsBrickExistOnIndex(int index)
        {
            bool isBrickExist = false;

            if (entities[index] != Entity.Null) isBrickExist = true;
            return isBrickExist;
        }

        private int CalcPositionToIndex(Vector3 targetPosition)
        {
            int tmpIdx = 0;
            tmpIdx += (int)Mathf.RoundToInt((targetPosition.x - oneSideLength / 2 + 0.5f) / oneSideLength);
            tmpIdx += (int)Mathf.RoundToInt((targetPosition.y - oneSideLength / 2 + 0.5f) / oneSideLength / oneSideLength);
            tmpIdx += (int)Mathf.RoundToInt((targetPosition.z - oneSideLength / 2 + 0.5f) / oneSideLength / oneSideLength / oneSideLength);

            return tmpIdx;
        }

        public Vector3 CalcIndexToPosition(int targetIndex)
        {
            int oneSideNumber = (int)Mathf.RoundToInt(1 / oneSideLength);
            int xIndexUnit = 1;
            int yIndexUnit = xIndexUnit * oneSideNumber;
            int zIndexUnit = yIndexUnit * oneSideNumber;

            int index_z = targetIndex / zIndexUnit;
            int index_y = (targetIndex - index_z * zIndexUnit) / yIndexUnit;
            int index_x = (targetIndex - index_z * zIndexUnit - index_y * yIndexUnit) / xIndexUnit;

            float zSideLength = index_z * oneSideLength;
            float ySideLength = index_y * oneSideLength;
            float xSideLength = index_x * oneSideLength;

            // position transformation from zero.
            float position_z = zSideLength - 0.5f + oneSideLength / 2;
            float position_y = ySideLength - 0.5f + oneSideLength / 2;
            float position_x = xSideLength - 0.5f + oneSideLength / 2;
            Vector3 targetPosition;
            targetPosition = new Vector3(position_x, position_y, position_z);

            return targetPosition;
        }

        private float ConvertPositionLocalToWorld(float positionValue)
        {
            return positionValue - ((1f + oneSideLength) / 2);
        }

        public void DeleteBrick(bool playSound)
        {
            int cursorID = entityManager.GetComponentData<BrickInfo>(cursorBrickEntity)._id;

            if (entities[cursorID] != Entity.Null)
            {
                entityManager.DestroyEntity(entities[cursorID]);
                entities[cursorID] = Entity.Null;
                totalBrickNumber--;
                _isSaved = false;
                if (playSound == true) PlayDeleteSound();
            }
        }
        public void DeleteBrick(int id)
        {
            if (entities[id] != Entity.Null)
            {
                entityManager.DestroyEntity(entities[id]);
                entities[id] = Entity.Null;
                totalBrickNumber--;
                _isSaved = false;
            }
            BrickSelectionManager.instance.DeleteSelectionBox(id);
        }

        /// <summary>
        /// Information of bricks
        /// </summary>
        public List<Entity> GetAllBricks()
        {
            List<Entity> list_entities = new List<Entity>();
            for (int i = 0; i < maxIdx; i++)
            {
                if (entities[i] != Entity.Null)
                {
                    list_entities.Add(entities[i]);
                }
            }
            return list_entities;
        }


        // get bricks info list async 

        public List<BrickInfo> List_BricksInfo = new List<BrickInfo>();
        public async Task GetBricksInfoAsync()
        {
            await Task.Run(() =>
            {
                BrickInfo tmpBrickInfo;

                int osn = EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber;
                int totalCubeNum = osn * osn * osn;
                for (int i = 0; i < totalCubeNum; i++)
                {
                    tmpBrickInfo = GetBrickInfo(i);
                    if (tmpBrickInfo._id >= 0)
                    {
                        List_BricksInfo.Add(tmpBrickInfo);
                    }
                }
            });
        }

        public IEnumerator GetBricksInfoCoroutine()
        {
            BrickInfo tmpBrickInfo;

            int osn = EditorUIManager.Instance.cubeDesigneInstance.oneSideNumber;
            int totalCubeNum = osn * osn * osn;
            for (int i = 0; i < totalCubeNum; i++)
            {
                tmpBrickInfo = GetBrickInfo(i);
                if (tmpBrickInfo._id >= 0)
                {
                    List_BricksInfo.Add(tmpBrickInfo);
                }
                if(i%2000 == 0)
                    yield return null;
            }

            DataManager.instance.SavingCubeDesignDataCoroutine = null;
            yield return null;
        }

        public BrickInfo GetBrickInfo(int index)
        {
            BrickInfo tmpBrickInfo = new BrickInfo();
            int _id = index;
            bool _isExist;
            string _meshName;
            string _matCategory;
            string _matName;
            float3 _rotateValue;
            float _scaleFactor;

            if (entities[_id] != Entity.Null)
            {
                _isExist = true;
                _meshName = entityManager.GetComponentData<BrickInfo>(entities[_id])._meshName;
                _matCategory = entityManager.GetComponentData<BrickInfo>(entities[_id])._matCategory;
                _matName = entityManager.GetComponentData<BrickInfo>(entities[_id])._matName;
                _rotateValue = entityManager.GetComponentData<BrickInfo>(entities[_id])._rotationValue;
                _scaleFactor = entityManager.GetComponentData<BrickInfo>(entities[_id])._scaleFactor;
            }
            else
            {
                _isExist = false;
                _meshName = "";
                _matCategory = "";
                _matName = "";
                _rotateValue = 0;
                _scaleFactor = 1.0f;
            }

            tmpBrickInfo._id = _id;
            tmpBrickInfo._isExist = _isExist;
            tmpBrickInfo._meshName = _meshName;
            tmpBrickInfo._matCategory = _matCategory;
            tmpBrickInfo._matName = _matName;
            tmpBrickInfo._rotationValue = _rotateValue;
            tmpBrickInfo._scaleFactor = _scaleFactor;

            return tmpBrickInfo;
        }

        /// <summary>
        /// ECS
        /// </summary>
        private void SetupECSPure()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;
            brickEntityArchtype = entityManager.CreateArchetype
            (
                typeof(BrickInfo),
                typeof(Translation),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(Scale),
                typeof(RenderBounds)
            );
            brickCursorEntityArchtype = entityManager.CreateArchetype
            (
                typeof(BrickInfo),
                typeof(Translation),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(Scale),
                typeof(RenderBounds),
                typeof(Collider),
                typeof(PhysicsDebugDisplayData)
            );
        }


        public int SetAutoPut(bool state)
        {
            int retCode = 0;

            if (autoDelete == true)
            {
                retCode = -1;
            }

            if (state == true)
            {
                autoPut = true;
                PutBrick(true);
            }
            else
            {
                autoPut = false;
            }
            return retCode;
        }

        public int SetAutoDelete(bool state)
        {
            int retCode = 0;

            if (autoPut == true)
            {
                retCode = -1;
            }

            if (state == true)
            {
                autoDelete = true;
                DeleteBrick(true);
            }
            else
            {
                autoDelete = false;
            }
            return retCode;
        }

        // effect sounds
        //==========================================================================
        public void PlayPutSound()
        {
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX("PutButtonSound");
        }
        public void PlayDeleteSound()
        {
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX("DeleteButtonSound");
        }

        // data
        //===========================================================================
        public void SetSavedFlag(bool isSaved)
        {
            _isSaved = isSaved;
        }
        public bool IsSaved()
        {
            return _isSaved;
        }

        public void DisableAllBricks()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != Entity.Null)
                    entityManager.AddComponent<Disabled>(entities[i]);
            }
        }
        public void EnableAllBricks()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != Entity.Null)
                    entityManager.RemoveComponent<Disabled>(entities[i]);
            }
        }
        public void EnableBricksOnXZPlane(int y)
        {
            int tmpIdx = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                tmpIdx = i;
                tmpIdx = tmpIdx % (oneSideNumber * oneSideNumber);
                if ((tmpIdx / oneSideNumber == y) &
                   (entityManager.Exists(entities[i]) == true))
                    entityManager.RemoveComponent<Disabled>(entities[i]);

            }
        }

        // debug
        void Debug_ShowBricksCollider()
        {
            foreach (var entity in entities)
            {
                entityManager.AddComponentData(entity, new Unity.Physics.Authoring.PhysicsDebugDisplayData
                {
                    DrawBroadphase = 0,
                    DrawColliders = 0,
                    DrawColliderAabbs = 0,
                    DrawColliderEdges = 1
                });
            }
        }

        // for testing
        public void FillOutWithBrick()
        {
            for (int i = 0; i < maxIdx; i++)
            {
                MoveCursorTo(i);
                PutBrick(false);
            }
        }
    }

    
}