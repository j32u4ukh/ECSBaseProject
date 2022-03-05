using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

public class MoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        // Allocator.TempJob: Because this NativeArray is temporarily exist while a job is running
        NativeArray<float3> wps = new NativeArray<float3>(
            GameDataManager.instance.wps,
            Allocator.TempJob
        );

        var jobHandle = Entities
               .WithName("MoveSystem")
               .ForEach((ref Translation position, ref Rotation rotation, ref TankData tankData) =>
               {
                   float3 heading = wps[tankData.current_wp] - position.Value;
                   heading.y = 0;
                   quaternion targetDirection = quaternion.LookRotation(heading, math.up());
                   rotation.Value = math.slerp(rotation.Value, targetDirection, deltaTime * tankData.rotationalSpeed);
                   position.Value += deltaTime * tankData.speed * math.forward(rotation.Value);

                   if (math.distance(position.Value, wps[tankData.current_wp]) < 1)
                   {
                       tankData.current_wp++;

                       if(tankData.current_wp >= wps.Length)
                       {
                           tankData.current_wp = 0;
                       }
                   }
               })
               .Schedule(inputDeps);

        // Give wps jobHandle to make sure it won't dispose until this job finished. 
        wps.Dispose(jobHandle);

        return jobHandle;
    }

}
