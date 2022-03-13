using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class FireSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {     
        if (Input.GetMouseButton(0))
        {
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;

            NativeArray<float3> bullet_spawn_points = new NativeArray<float3>(
                GameDataManager.bullet_points,
                Allocator.TempJob
            );

            Entities.WithoutBurst()
                    .WithStructuralChanges()
                    .WithName("FireSystem")
                    .ForEach((ref PhysicsVelocity physics, ref Translation position, ref Rotation rotation, ref CharacterData character) =>
                    {
                        foreach (float3 bullet_spawn_point in bullet_spawn_points)
                        {
                            var instance = manager.Instantiate(character.bullet_entity);

                            manager.SetComponentData(instance, new Translation { Value = position.Value + math.mul(rotation.Value, bullet_spawn_point) });
                            manager.SetComponentData(instance, new Rotation { Value = rotation.Value });
                            manager.SetComponentData(instance, new LifeTimeData { value = 2f });
                            manager.SetComponentData(instance, new BulletData { 
                                speed = 50f,
                                collision_effect = new float3(0, 1000, 0)
                            });
                        }
                    })
                    .Run();

            bullet_spawn_points.Dispose();
        }

        return inputDeps;
    }
}
