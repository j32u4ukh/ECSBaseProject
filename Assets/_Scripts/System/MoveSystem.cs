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
               .ForEach((ref Translation position, ref Rotation rotation, ref ShipData ship_data) =>
               {
                   float3 heading = wps[ship_data.current_wp] - position.Value;
                   //heading.y = 0;
                   quaternion targetDirection = quaternion.LookRotation(heading, math.up());
                   rotation.Value = math.slerp(rotation.Value, targetDirection, deltaTime * ship_data.rotationSpeed);
                   position.Value += deltaTime * ship_data.speed * math.forward(rotation.Value);

                   if (math.distance(position.Value, wps[ship_data.current_wp]) < 3)
                   {
                       ship_data.current_wp++;

                       if(ship_data.current_wp >= wps.Length)
                       {
                           ship_data.current_wp = 0;
                       }
                   }
               })
               .Schedule(inputDeps);

        // Give wps jobHandle to make sure it won't dispose until this job finished. 
        wps.Dispose(jobHandle);

        return jobHandle;
    }

}
