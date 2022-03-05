using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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
                    float3 direction = GameDataManager.instance.wps[ship_data.current_wp] - position.Value;

                    // 利用 餘弦定理 計算兩向量夾角(angle)
                    float cos = math.dot(math.forward(rotation.Value), direction);
                    float len_a = math.length(math.forward(rotation.Value));
                    float distance = math.length(direction);
                    float angle = math.acos(cos / (len_a * distance));

                    if((angle < math.radians(5)) && (distance < 100.0f))
                    {
                        foreach (float3 bullet_spawn_point in bullet_spawn_points)
                        {
                            var instance = manager.Instantiate(ship_data.bullet_entity);

                            manager.SetComponentData(instance, new Translation { Value = position.Value + math.mul(rotation.Value, bullet_spawn_point) });
                            manager.SetComponentData(instance, new Rotation { Value = rotation.Value });
                            manager.SetComponentData(instance, new LifeTimeData { value = 1f });
                            manager.SetComponentData(instance, new BulletData { 
                                target = ship_data.current_wp,
                                explosion_entity = ship_data.explosion_entity
                            });
                        }
                    }                    
                })
                .Run();

        bullet_spawn_points.Dispose();

        return inputDeps;
    }
}
