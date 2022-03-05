using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

public class ShipMovingSystem : JobComponentSystem
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
               .WithName("ShipMovingSystem")
               .ForEach((ref Translation position, ref Rotation rotation, ref ShipData ship_data) =>
               {
                   float distance = math.distance(position.Value, wps[ship_data.current_wp]);

                   if (distance < 60f)
                   {
                       ship_data.approach = false;
                   }
                   else if(distance > 500f)
                   {
                       ship_data.approach = true;
                   }

                   float3 heading;

                   // 接近目標
                   if (ship_data.approach)
                   {
                       heading = wps[ship_data.current_wp] - position.Value;
                   }

                   // 遠離目標
                   else
                   {
                       heading = position.Value - wps[ship_data.current_wp];
                   }

                   quaternion targetDirection = quaternion.LookRotation(heading, math.up());
                   rotation.Value = math.slerp(rotation.Value, targetDirection, deltaTime * ship_data.rotationSpeed);
                   position.Value += deltaTime * ship_data.speed * math.forward(rotation.Value);
               })
               .Schedule(inputDeps);

        // Give wps jobHandle to make sure it won't dispose until this job finished. 
        wps.Dispose(jobHandle);

        return jobHandle;
    }

}
