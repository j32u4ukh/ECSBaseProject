using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

public class RaycastHitJob : MonoBehaviour
{
    public float distance = 10.0f;
    public Vector3 direction = Vector3.forward;

    public bool collect_all_hits = false;
    public bool draw_surface_normals = true;

    RaycastInput raycast_input;
    float3 origin;
    float3 ray_direction;

    NativeList<Unity.Physics.RaycastHit> raycast_hits;
    NativeList<DistanceHit> distance_hits;

    BuildPhysicsWorld physics_world;
    StepPhysicsWorld step_world;

    EntityManager manager;

    // Start is called before the first frame update
    void Start()
    {
        // Allocator.Persistent: 持續存在記憶體當中，直到程式結束
        raycast_hits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Persistent);
        distance_hits = new NativeList<DistanceHit>(Allocator.Persistent);

        physics_world = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        step_world = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        step_world.FinalJobHandle.Complete();

        origin = transform.position;
        ray_direction = (transform.rotation * direction) * distance;

        raycast_hits.Clear();
        distance_hits.Clear();

        raycast_input = new RaycastInput
        {
            Start = origin,
            End = origin + ray_direction,
            Filter = CollisionFilter.Default
        };

        JobHandle handle = new RaycastJob
        {
            raycast_input = raycast_input,
            raycast_hits = raycast_hits,
            collect_all_hits = collect_all_hits,
            world = physics_world.PhysicsWorld

        }.Schedule();

        handle.Complete();

        foreach (Unity.Physics.RaycastHit hit in raycast_hits.ToArray())
        {
            var entity = physics_world.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            manager.DestroyEntity(entity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycast_input.Start, raycast_input.End - raycast_input.Start);

        if (raycast_hits.IsCreated)
        {
            foreach(Unity.Physics.RaycastHit hit in raycast_hits.ToArray())
            {
                Assert.IsTrue(0 <= hit.RigidBodyIndex && hit.RigidBodyIndex < physics_world.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(raycast_input.Start, hit.Position - raycast_input.Start);
                Gizmos.DrawSphere(hit.Position, 0.02f);

                if (draw_surface_normals)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(hit.Position, hit.SurfaceNormal);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (raycast_hits.IsCreated)
        {
            raycast_hits.Dispose();
        }

        if (distance_hits.IsCreated)
        {
            distance_hits.Dispose();
        }
    }
}

public struct RaycastJob : IJob
{
    [ReadOnly] public PhysicsWorld world;

    public RaycastInput raycast_input;
    public NativeList<Unity.Physics.RaycastHit> raycast_hits;
    public bool collect_all_hits;

    public void Execute()
    {
        if (collect_all_hits)
        {
            world.CastRay(raycast_input, ref raycast_hits);
        }
        else if (world.CastRay(raycast_input, out Unity.Physics.RaycastHit hit))
        {
            raycast_hits.Add(hit);
        }
    }
}