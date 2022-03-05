using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

public class BulletMovingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        var jobHandle = Entities
               .WithName("BulletMovingSystem")
               .ForEach((ref Translation position, ref Rotation rotation, ref BulletData bullet_data) =>
               {
                   position.Value += deltaTime * 100f * math.forward(rotation.Value);
               })
               .Schedule(inputDeps);

        jobHandle.Complete();

        Entities.WithoutBurst().WithStructuralChanges()
                .ForEach((Entity entity, ref Translation position, ref Rotation rotation, ref BulletData bullet, ref LifeTimeData life) =>
                {
                    float3 target = GameDataManager.instance.wps[bullet.target];
                    float distance = math.distance(position.Value, target);

                    if(distance < 27f)
                    {
                        life.value = 0f;

                        if (UnityEngine.Random.Range(0, 1000) < 50)
                        {
                            Entity instance = EntityManager.Instantiate(bullet.explosion_entity);
                            EntityManager.SetComponentData(instance, new Translation { Value = position.Value});
                            EntityManager.SetComponentData(instance, new Rotation { Value = rotation.Value});
                            EntityManager.SetComponentData(instance, new LifeTimeData { value = 0.5f });
                        }
                    }
                })
                .Run();

        return jobHandle;
    }

}
