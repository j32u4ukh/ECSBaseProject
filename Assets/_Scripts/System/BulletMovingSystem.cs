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

                   //quaternion targetDirection = quaternion.LookRotation(bullet_data.destination, math.up());
                   //position.Value += deltaTime * bullet_data.speed * math.forward(rotation.Value);
               })
               .Schedule(inputDeps);
        
        return jobHandle;
    }

}
