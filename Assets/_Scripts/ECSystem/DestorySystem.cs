using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BulletMovingSystem))]
public class DestorySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;

        Entities.WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, ref LifeTimeData life) =>
                {
                    life.value -= dt;

                    if (life.value <= 0f)
                    {
                        EntityManager.DestroyEntity(entity);
                    }

                })
                .Run();

        return inputDeps;
    }
}
