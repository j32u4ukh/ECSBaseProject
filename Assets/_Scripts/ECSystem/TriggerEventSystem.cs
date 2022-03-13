using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class TriggerEventSystem : JobComponentSystem
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
        JobHandle handle = new TriggerEventJob
        {
            trigger_group = GetComponentDataFromEntity<TriggerData>(),
            velocity_group = GetComponentDataFromEntity<PhysicsVelocity>()
        }.Schedule(step_physics_world.Simulation, 
                   ref physics_world.PhysicsWorld, 
                   inputDeps);

        return handle;
    }
}

struct TriggerEventJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentDataFromEntity<TriggerData> trigger_group;

    // Here will get not only dynamic but also kinematic
    public ComponentDataFromEntity<PhysicsVelocity> velocity_group;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity entity_a = triggerEvent.Entities.EntityA;
        Entity entity_b = triggerEvent.Entities.EntityB;

        bool a_is_trigger = trigger_group.Exists(entity_a);
        bool b_is_trigger = trigger_group.Exists(entity_b);

        if(a_is_trigger && b_is_trigger)
        {
            return;
        }

        bool a_is_dynamic = velocity_group.Exists(entity_a);
        bool b_is_dynamic = velocity_group.Exists(entity_b);

        // 排除和 static 的接觸
        bool condition1 = a_is_trigger && !b_is_dynamic;
        bool condition2 = b_is_trigger && !a_is_dynamic;

        if(condition1 || condition2)
        {
            return;
        }

        var trigger_entity = a_is_trigger ? entity_a : entity_b;
        var dynamic_entity = a_is_trigger ? entity_b : entity_a;

        var trigger = trigger_group[trigger_entity];
        var component = velocity_group[dynamic_entity];
        component.Linear += trigger.trigger_effect;
        velocity_group[dynamic_entity] = component;
    }
}
