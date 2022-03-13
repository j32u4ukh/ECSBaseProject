using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class BulletCollisionEventSystem : JobComponentSystem
{
    BuildPhysicsWorld physics_world;
    StepPhysicsWorld step_physics_world;

    protected override void OnCreate()
    {
        physics_world = World.GetOrCreateSystem<BuildPhysicsWorld>();
        step_physics_world = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle handle = new CollisionEventJob
        {
            bullet_group = GetComponentDataFromEntity<BulletData>(),
            death_group = GetComponentDataFromEntity<DeathData>()
        }.Schedule(step_physics_world.Simulation, 
                   ref physics_world.PhysicsWorld, 
                   inputDeps);

        handle.Complete();

        return inputDeps;
    }
}

struct CollisionEventJob : ICollisionEventsJob
{
    [ReadOnly] public ComponentDataFromEntity<BulletData> bullet_group;
    public ComponentDataFromEntity<DeathData> death_group;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entity_a = collisionEvent.Entities.EntityA;
        Entity entity_b = collisionEvent.Entities.EntityB;

        bool a_is_bullet = bullet_group.Exists(entity_a);
        bool b_is_bullet = bullet_group.Exists(entity_b);

        bool a_is_target = death_group.Exists(entity_a);
        bool b_is_target = death_group.Exists(entity_b);

        if(a_is_bullet && b_is_target)
        {
            DeathData target = death_group[entity_b];
            target.is_dead = true;
            death_group[entity_b] = target;
        }
        else if (b_is_bullet && a_is_target)
        {
            DeathData target = death_group[entity_a];
            target.is_dead = true;
            death_group[entity_a] = target;
        }
    }
}
