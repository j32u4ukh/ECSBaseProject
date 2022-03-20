using Unity.Entities;
using Unity.Jobs;

public class LifeTimeSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;

        Entities.WithoutBurst()
                .WithStructuralChanges()
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