using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;


public class Test_MeshDataManager : MonoBehaviour
{
    [SerializeField] GameObject stair2;
    [SerializeField] GameObject stair3;
    [SerializeField] Transform spawningPool;

    [SerializeField] Material mat;
    private World defaultWorld;
    private EntityManager entityManager;
    private EntityArchetype entityArcheType;

    private GameObject go;
    private Entity e;
    private void Awake()
    {
        ECSSetup();
    }
    private void Start()
    {
        SpawnStair2();
        SpawnStair2Entity();
    }

    private void ECSSetup()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;
        entityArcheType = entityManager.CreateArchetype
        (
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Scale),
            typeof(RenderBounds)
        );
    }

    private void SpawnStair2()
    {
        go= Instantiate(stair2, spawningPool);
    }

    private void SpawnStair2Entity()
    {
        e = entityManager.CreateEntity(entityArcheType);
        entityManager.AddSharedComponentData(e, new RenderMesh
        {
            mesh = go.GetComponentInChildren<Mesh>(),
            material = mat
        }
        ) ;
        entityManager.AddComponentData(e, new Translation
        {
            Value = new float3(spawningPool.position.x, spawningPool.position.y, spawningPool.position.z),
        });
        entityManager.AddComponentData(e, new Rotation
        {
            Value = Quaternion.identity
        });
        entityManager.AddComponentData(e, new Scale { Value = 1f });
        entityManager.AddComponentData(e, new LocalToWorld { });
    }


}
