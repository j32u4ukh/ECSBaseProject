using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

//[UpdateAfter(typeof(LifeTimeSystem))]
public class DeathSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst()
                .WithStructuralChanges()
                .WithName("DeathSystem")
                .ForEach((Entity entity, ref DeathData death) =>
                {
                    if (death.is_dead)
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                })
                .Run();

        return inputDeps;
    }
}