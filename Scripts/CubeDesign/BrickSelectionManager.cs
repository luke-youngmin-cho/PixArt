using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using System;
using System.Collections.Generic;

namespace DOTS
{
    public class BrickSelectionManager : MonoBehaviour
    {
        static public BrickSelectionManager instance;
        private void Awake()
        {
            if (instance == null) instance = this;
        }

        private bool refreshed;
        private int maxIdx;
        private World defaultWorld;
        private EntityManager entityManager;
        private EntityArchetype entityArchtype;
        [SerializeField] private Mesh boxMesh;
        [SerializeField] private Material boxMat;
        private float scale;

        // info
        public int selectedNum { get { return list_id.Count; } }
        private void Start()
        {
            SetupECSPure();
        }
        private Entity[] entities;
        public List<int> list_id = new List<int>();

        public void SetupECSPure()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;
            entityArchtype = entityManager.CreateArchetype
            (
                //typeof(BrickInfo),
                typeof(Translation),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(Scale),
                typeof(RenderBounds)
            );
        }
        public void Refresh()
        {
            maxIdx = EditorUIManager.Instance.cubeDesigneInstance.maxIdx;
            scale = EditorUIManager.Instance.cubeDesigneInstance.oneSideLength / 2;
            entities = new Entity[maxIdx + 1];
            refreshed = true;
        }

        private void CreateSelectionBox(int id)
        {
            if (refreshed == false) return;
            if (EditorUIManager.Instance.cubeDesigneInstance == null) return;

            Entity box = entityManager.CreateEntity(entityArchtype);
            entityManager.AddComponentData(box, new Translation
            {
                Value = EditorUIManager.Instance.cubeDesigneInstance.CalcIndexToPosition(id)
            }
            );
            entityManager.AddComponentData(box, new Rotation
            {
                Value = Quaternion.identity
            });
            entityManager.AddSharedComponentData(box, new RenderMesh
            {
                mesh = boxMesh,
                material = boxMat,
            });
            entityManager.AddComponentData(box, new Scale { Value = scale });
            entityManager.AddComponentData(box, new LocalToWorld { });
            Debug.Log("select box created!");
            entities[id] = box;
        }

        public void DeleteSelectionBox(int id)
        {
            if (refreshed == false) return;
            if (EditorUIManager.Instance.cubeDesigneInstance == null) return;
            if (list_id.Contains(id) == false) return;
            entityManager.DestroyEntity(entities[id]);
            list_id.Remove(id);
        }

        public void Select(int id)
        {
            if (refreshed == false) return;
            // get cursor info
            if (list_id.Contains(id) == false)
            {
                list_id.Add(id);
                Debug.Log($"you selected {list_id.Count} brick(s)");
                CreateSelectionBox(id);
            }
            else
            {
                list_id.Remove(id);
                Debug.Log($"you Deselected {list_id.Count} brick(s)");
                DeleteSelectionBox(id);
            }
        }
        public void SelectAll()
        {
            int id;

            foreach (var brick in EditorUIManager.Instance.cubeDesigneInstance.GetAllBricks())
            {
                id = entityManager.GetComponentData<BrickInfo>(brick)._id;

                if (list_id.Contains(id) == false)
                {
                    list_id.Add(id);
                    CreateSelectionBox(id);
                }
            }
        }
        public void Deselect()
        {
        }
        public void DeselectAll()
        {
        }
    }

}
