using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;

namespace udemy
{
    public class MoveSystem : JobComponentSystem
    {
        /// <summary>
        /// 處理當前 Job 對其他 Job 的依賴關係，當 inputDeps 還沒結束，當前 Job 無法開始。
        /// 但此例中只有一個 Job，沒有依賴關係。
        /// </summary>
        /// <param name="inputDeps">先於當前 Job 被執行的其他 Job</param>
        /// <returns></returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // NOTE: 暫時直接返回 inputDeps，不進行下方處理，避免影響其他部分的教學
            //return inputDeps;

            float deltaTime = Time.DeltaTime;
            float speed = 20.0f;
            //float rotationalSpeed = 0.5f;
            float3 targetLocation = new float3(0, 0, 0);

            // WithName: 將所有 Entity 放入名為 MoveSystem 的系統中
            // 尋找有 Translation, Rotation 和 NonUniformScale 組件的 Entity 並對其做處理，只有其中幾項組件的不會被考慮
            var jobHandle = Entities.WithName("MoveSystem")
                                    .ForEach((ref Translation position, ref Rotation rotation) =>
                                    {
                                        //position.Value += 0.01f * math.up();

                                        //if (position.Value.y > 100)
                                        //{
                                        //    position.Value.y = 0;
                                        //}

                                        //float3 heading = targetLocation - position.Value;
                                        float3 pivot = targetLocation;
                                        //heading.y = 0f;

                                        // 距離越遠，速度越慢
                                        float rotationalSpeed = deltaTime * speed * 1f / math.distance(pivot, position.Value);
                                        //quaternion targetDirection = quaternion.LookRotation(heading, math.up());
                                        //rotation.Value = math.slerp(rotation.Value, targetDirection, deltaTime * rotationalSpeed);

                                        //position.Value.z += 0.1f;
                                        //position.Value += deltaTime * speed * math.forward(rotation.Value);
                                        //position.Value += deltaTime * speed * heading;
                                        position.Value = math.mul(quaternion.AxisAngle(new float3(0, 1, 0), rotationalSpeed),
                                                                  (position.Value - pivot) + pivot);

                                        //if (position.Value.z > 50)
                                        //{
                                        //    position.Value.z = -50;
                                        //}
                                    })
                                    .Schedule(inputDeps);

            return jobHandle;
        }
    }
}
