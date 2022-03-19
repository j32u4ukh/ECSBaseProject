using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;

public class ZombieMovingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float delta_time = Time.DeltaTime;

        NativeArray<float3> towers = new NativeArray<float3>(
            DataManager.towers,
            Allocator.TempJob
        );

        int next_target = UnityEngine.Random.Range(0, towers.Length);

        var jobHandle = Entities
               .WithName("ZombieMovingSystem")
               .ForEach((ref ZombieData zombie, ref PhysicsVelocity physics, ref PhysicsMass mass, ref Translation position, ref Rotation rotation) =>
               {
                   float distance = math.distance(position.Value, towers[zombie.target]);

                   if(distance < 5)
                   {
                       zombie.target = next_target;
                   }

                   mass.InverseInertia[0] = 0;
                   mass.InverseInertia[1] = 0;
                   mass.InverseInertia[2] = 0;

                   float3 heading = towers[zombie.target] - position.Value;
                   quaternion q = quaternion.LookRotation(heading, math.up());
                   rotation.Value = math.slerp(rotation.Value, q, delta_time * zombie.rotate_speed);
                   physics.Linear = delta_time * zombie.speed * math.forward(rotation.Value);
                   //rotation.Value = math.mul(math.normalize(rotation.Value),
                   //                          quaternion.AxisAngle(math.up(), delta_time * zombie.rotate_speed));
               })
               .Schedule(inputDeps);

        towers.Dispose(jobHandle);
        jobHandle.Complete();

        return jobHandle;
    }

}