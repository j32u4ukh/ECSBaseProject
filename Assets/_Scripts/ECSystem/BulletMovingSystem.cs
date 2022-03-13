using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;

public class BulletMovingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        var jobHandle = Entities
               .WithName("BulletMovingSystem")
               .ForEach((ref PhysicsVelocity physics, ref Translation position, ref Rotation rotation, ref BulletData bullet) =>
               {
                   //position.Value += deltaTime * bullet.speed * math.forward(rotation.Value);

                   physics.Angular = float3.zero;
                   physics.Linear += deltaTime * bullet.speed * math.forward(rotation.Value);

               })
               .Schedule(inputDeps);

        jobHandle.Complete();

        return jobHandle;
        //return inputDeps;
    }

}