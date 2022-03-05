using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

public class ShootSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entities.WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, ref Translation position, ref Rotation rotation, ref ShipData ship_data) =>
                {
                    var instance = manager.Instantiate(ship_data.bullet_entity);

                    manager.SetComponentData(instance, new Translation { Value = position.Value });
                    manager.SetComponentData(instance, new Rotation { Value = rotation.Value });

                })
                .Run();

        return inputDeps;
    }
}
