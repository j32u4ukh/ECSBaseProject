using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;

namespace udemy
{
    public class TankSystem : JobComponentSystem
    {
        /// <summary>
        /// 處理當前 Job 對其他 Job 的依賴關係，當 inputDeps 還沒結束，當前 Job 無法開始。
        /// 但此例中只有一個 Job，沒有依賴關係。
        /// </summary>
        /// <param name="inputDeps">先於當前 Job 被執行的其他 Job</param>
        /// <returns></returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // WithName: 將所有 Entity 放入名為 MoveSystem 的系統中
            // 尋找有 Translation, Rotation 和 NonUniformScale 組件的 Entity 並對其做處理，只有其中幾項組件的不會被考慮
            var jobHandle = Entities.WithName("MoveSystem")
                                    .ForEach((ref Translation position, ref Rotation rotation, ref TankData tankData) =>
                                    {

                                    })
                                    .Schedule(inputDeps);

            return jobHandle;
        }
    }
}
