using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Rendering;

namespace udemy
{
    public class CreateCapsuleSystem : JobComponentSystem
    {
        //protected override void OnCreate()
        //{
        //    base.OnCreate();

        //    int i, n_sheep = 100;
        //    float s;
        //    float3 translation;

        //    for (i = 0; i < n_sheep; i++)
        //    {
        //        s = UnityEngine.Random.Range(10, 20);
        //        //s = 10;
        //        translation = new float3(UnityEngine.Random.Range(-10, 10),
        //                                 0,
        //                                 UnityEngine.Random.Range(-10, 10));
        //        createEntity(translation: translation, scale: new float3(s, s, s));
        //    }
        //}

        void createEntity(float3 translation, float3 scale)
        {
            var instance = EntityManager.CreateEntity(
                               ComponentType.ReadOnly<LocalToWorld>(),
                               ComponentType.ReadWrite<Translation>(),
                               ComponentType.ReadWrite<Rotation>(),
                               ComponentType.ReadOnly<NonUniformScale>(),
                               ComponentType.ReadOnly<RenderMesh>()
                           );

            //EntityManager.SetComponentData(instance, new LocalToWorld
            //{
            //    Value = new float4x4(rotation: quaternion.identity,
            //                         translation: translation)
            //});
            EntityManager.SetComponentData(instance, new Translation { Value = translation });
            EntityManager.SetComponentData(instance, new Rotation { Value = new quaternion(0, 0, 0, 0) });
            EntityManager.SetComponentData(instance, new NonUniformScale { Value = scale });

            var rHolder = Resources.Load<GameObject>("ResourceHolder").GetComponent<ResourceHolder>();
            EntityManager.SetSharedComponentData(instance, new RenderMesh
            {
                mesh = rHolder.theMesh,
                material = rHolder.theMaterial
            });
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}
