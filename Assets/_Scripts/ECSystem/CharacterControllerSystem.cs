using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Rendering;
using Unity.Physics;

public class CharacterControllerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float delta_time = Time.DeltaTime;
        float angular = Input.GetAxis("Horizontal");
        float linear = Input.GetAxis("Vertical");

        var handle = Entities.WithName("CharacterControllerSystem")
                             .ForEach((ref PhysicsVelocity physics, ref Rotation rotation, ref CharacterData player) =>
                             {
                                 if(linear == 0f)
                                 {
                                     physics.Linear = float3.zero;
                                 }
                                 else
                                 {
                                     physics.Linear += linear * delta_time * player.speed * math.forward(rotation.Value);
                                 }

                                 //physics.Angular += new float3(0, angular * 0.1f, 0);
                                 rotation.Value = math.mul(math.normalize(rotation.Value), 
                                                           quaternion.AxisAngle(math.up(), delta_time * angular));
                             })
                             .Schedule(inputDeps);
        handle.Complete();

        return inputDeps;
    }
}
