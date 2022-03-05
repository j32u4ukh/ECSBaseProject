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
        
        NativeArray<float3> bullet_spawn_points = new NativeArray<float3>(
            GameDataManager.instance.gun_locations,
            Allocator.TempJob
        );

        Entities.WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, ref Translation position, ref Rotation rotation, ref ShipData ship_data) =>
                {
                    foreach(float3 bullet_spawn_point in bullet_spawn_points)
                    {
                        var instance = manager.Instantiate(ship_data.bullet_entity);

                        manager.SetComponentData(instance, new Translation { Value = position.Value + math.mul(rotation.Value, bullet_spawn_point) });
                        manager.SetComponentData(instance, new Rotation { Value = rotation.Value });
                    }
                })
                .Run();

        bullet_spawn_points.Dispose();

        return inputDeps;
    }
}
