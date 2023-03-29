using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class ObjectECS : MonoBehaviour
{
    [SerializeField]
    private Mesh satelliteMesh;

    [SerializeField]
    private Material satelliteMaterial;

    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype objectArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld),
            typeof(ObjectData)
        );

        Entity objectEntity = entityManager.CreateEntity(objectArchetype);
        entityManager.SetSharedComponentData(
            objectEntity,
            new RenderMesh
            {
                mesh = satelliteMesh,
                material = satelliteMaterial,
                layerMask = 1
            }
        );
    }
}

struct ObjectData : IComponentData
{
    // [SerializeField]
    // public SatelliteUtils.SatelliteData satelliteData;
}

partial class ObjectMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .ForEach(
                (ref Translation position, in ObjectData objectData) =>
                {
                    // position.Value = (Unity.Mathematics.float3)
                    //     objectData.satelliteData
                    //         .UpdateSatelliteState(SatelliteUtils.GetJulianDate(System.DateTime.Now))
                    //         .Position;
                }
            )
            .ScheduleParallel();
    }
}
