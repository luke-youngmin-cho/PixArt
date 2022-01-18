using System.Collections;
using System.Collections.Generic;
using DOTS;
using UnityEngine.EventSystems;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

public class CubeDesignRayCast : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler
{
    public static CubeDesignRayCast instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;
    }

    // CubeDesign info
    CubeDesign _cubeDesign;
    // Raycasting members
    [HideInInspector] public bool enable = false;
    [SerializeField] Camera cam;
    Vector2 cursor;
    NativeList<RaycastHit> RaycastHits;
    RaycastInput RaycastInput;

    // world
    World defaultWorld;
    EntityManager entityManager;

    [BurstCompile]
    struct RaycastJob : IJob
    {
        public RaycastInput RaycastInput;
        public NativeList<RaycastHit> RaycastHits;
        [ReadOnly] public PhysicsWorld World;

        public void Execute()
        {
            World.CastRay(RaycastInput, ref RaycastHits);
            //Debug.Log($"raycasted count : {RaycastHits.Length}");
        }
    }
    
    private void Start()
    {
        RaycastHits = new NativeList<RaycastHit>(Allocator.Persistent);
    }
    private void OnDestroy()
    {
        RaycastHits.Dispose();
    }
    public void OnDrag(PointerEventData eventData)
    {
        enable = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        enable = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(enable)
            ClickEvent();
    }
    private void ClickEvent()
    {
        ref PhysicsWorld world = ref World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
        Entity targetEntity;
        RaycastHits.Clear();

        cursor = Input.mousePosition;
        UnityEngine.Ray ray = cam.ScreenPointToRay(cursor);
        //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 1f);
        RaycastInput = new RaycastInput
        {
            Start = ray.origin,
            End = ray.origin + ray.direction * 100f,
            Filter = CollisionFilter.Default
        };

        new RaycastJob
        {
            RaycastInput = RaycastInput,
            RaycastHits = RaycastHits,
            World = world
        }.Schedule().Complete();


        int closestIdx = 0;
        float prevDistance = 0;
        float currentDistance = 0;
        Vector3 entityVector = new Vector3(0, 0, 0);

        if(RaycastHits.Length > 0)
        {
            for (int i = 0; i < RaycastHits.Length ; i++)
            {
                targetEntity = RaycastHits[i].Entity;
                entityVector = entityManager.GetComponentData<Translation>(targetEntity).Value;

                currentDistance = Vector3.Distance(ray.origin, entityVector);
                if (i == 0) 
                    prevDistance = currentDistance;

                if (currentDistance <= prevDistance)
                {
                    prevDistance = currentDistance;
                    closestIdx = i;
                }
            }

            if (entityManager.Exists(RaycastHits[closestIdx].Entity))
                _cubeDesign.MoveCursorTo(RaycastHits[closestIdx].Entity);
        }
    }

    public void SetCubeDesign(CubeDesign cubeDesign)
    {
        _cubeDesign = cubeDesign;
    }
}
