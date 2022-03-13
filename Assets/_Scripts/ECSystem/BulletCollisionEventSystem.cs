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
            velocity_group = GetComponentDataFromEntity<PhysicsVelocity>()
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
    public ComponentDataFromEntity<PhysicsVelocity> velocity_group;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entity_a = collisionEvent.Entities.EntityA;
        Entity entity_b = collisionEvent.Entities.EntityB;

        bool a_is_bullet = bullet_group.Exists(entity_a);
        bool b_is_bullet = bullet_group.Exists(entity_b);

        bool a_is_target = velocity_group.Exists(entity_a);
        bool b_is_target = velocity_group.Exists(entity_b);

        if(a_is_bullet && b_is_target)
        {
            PhysicsVelocity target = velocity_group[entity_b];
            target.Linear = new float3(0, 1000, 0);
            velocity_group[entity_b] = target;
        }
        else if (b_is_bullet && a_is_target)
        {
            PhysicsVelocity target = velocity_group[entity_a];
            target.Linear = new float3(0, 1000, 0);
            velocity_group[entity_a] = target;
        }
    }
}
