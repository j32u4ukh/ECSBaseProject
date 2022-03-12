using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ParticleSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float delta_time = Time.DeltaTime;

        var jobHandle = Entities
               .WithName("ParticleSystem")
               .ForEach((ref NonUniformScale scale, ref ParticleData particle) =>
               {
                   particle.alive_time += delta_time;
                   scale.Value = particle.alive_time * 0.8f;
               })
               .Schedule(inputDeps);

        jobHandle.Complete();

        return inputDeps;
    }
}
