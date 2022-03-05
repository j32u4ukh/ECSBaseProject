using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BulletMovingSystem))]
public class TimeDestorySystem : JobComponentSystem
{
    //EndSimulationEntityCommandBufferSystem buffer;

    //protected override void OnCreate()
    //{
    //    buffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    //}

    //protected override JobHandle OnUpdate(JobHandle inputDeps)
    //{
    //    EntityCommandBuffer.Concurrent concurrent = buffer.CreateCommandBuffer().ToConcurrent();
        
    //    CullingJob job = new CullingJob() {
    //        concurrent = concurrent,
    //        delta_time = Time.DeltaTime
    //    };

    //    JobHandle handler = job.Schedule(this, inputDeps);
    //    buffer.AddJobHandleForProducer(handler);

    //    handler.Complete();

    //    return inputDeps;
    //}

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;

        Entities.WithoutBurst().WithStructuralChanges()
                .ForEach((Entity entity, ref LifeTimeData life) =>
                {
                    life.value -= dt;

                    if (life.value <= 0f) 
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                    
                })
                .Run();

        return inputDeps;
    }
}

struct CullingJob : IJobForEachWithEntity<LifeTimeData>
{
    public EntityCommandBuffer.Concurrent concurrent;
    public float delta_time;

    public void Execute(Entity entity, int index, ref LifeTimeData life)
    {
        life.value -= delta_time;

        if (life.value <= 0f)
        {
            concurrent.DestroyEntity(index, entity);
        }
    }
}
