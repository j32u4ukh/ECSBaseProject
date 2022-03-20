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

public class PointDistanceHitJob : MonoBehaviour
{
    public float distance = 10.0f;

    public bool collect_all_hits = false;
    public bool draw_surface_normals = true;

    public AudioSource fire; 
    public AudioSource splat;
    public ParticleSystem shoot;

    public GameObject blood_prefab;
    BlobAssetStore store;
    Entity blood_entity;
    //Entity blood;

    BuildPhysicsWorld physics_world;
    StepPhysicsWorld step_world;
    EntityManager manager;

    NativeList<DistanceHit> distance_hits;
    PointDistanceInput point_distance_input;
    float3 origin;

    Entity closest_entity;
    Vector3 locked_on;
    float d;

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        store = new BlobAssetStore();

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        blood_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(blood_prefab, settings);

        distance_hits = new NativeList<DistanceHit>(Allocator.Persistent);

        physics_world = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        step_world = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();

        shoot.Stop();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        step_world.FinalJobHandle.Complete();
        origin = transform.position;

        distance_hits.Clear();

        point_distance_input = new PointDistanceInput
        {
            Position = origin,
            MaxDistance = distance,
            Filter = CollisionFilter.Default
        };

        JobHandle handle = new PointDistanceJob
        {
            point_distance_input = point_distance_input,
            distance_hits = distance_hits,
            collect_all_hits = collect_all_hits,
            world = physics_world.PhysicsWorld
        }.Schedule();

        handle.Complete();

        if (!manager.Exists(closest_entity))
        {
            d = Mathf.Infinity;
            locked_on = Vector3.zero;

            foreach (DistanceHit hit in distance_hits.ToArray())
            {
                Assert.IsTrue(0 <= hit.RigidBodyIndex && hit.RigidBodyIndex < physics_world.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                var entity = physics_world.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                bool is_zombie = manager.HasComponent<ZombieData>(entity);

                if((hit.Distance < d) && is_zombie)
                {
                    d = hit.Distance;
                    locked_on = manager.GetComponentData<Translation>(entity).Value;
                    closest_entity = entity;

                    Invoke("destoryClosest", 2);
                    fire.Play();
                    shoot.Play();
                }
            }
        }
        else
        {
            locked_on = manager.GetComponentData<Translation>(closest_entity).Value;
        }

        transform.LookAt(locked_on);
    }

    private void OnDrawGizmos()
    {

        if (distance_hits.IsCreated)
        {
            foreach (DistanceHit hit in distance_hits.ToArray())
            {
                Assert.IsTrue(0 <= hit.RigidBodyIndex && hit.RigidBodyIndex < physics_world.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(transform.position, hit.Position - (float3)transform.position);
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
        if (distance_hits.IsCreated)
        {
            distance_hits.Dispose();
        }

        store.Dispose();
    }

    void destoryClosest()
    {
        if (manager.HasComponent<ZombieData>(closest_entity))
        {
            float3 position = manager.GetComponentData<Translation>(closest_entity).Value;

            for (int i = 0; i < 100; i++)
            {
                position += (float3)UnityEngine.Random.insideUnitSphere * 0.1f;
                Entity blood = manager.Instantiate(blood_entity);
                manager.SetComponentData(blood, new Translation { Value = position });
            }

            manager.DestroyEntity(closest_entity);
            splat.Play();
            fire.Stop();
            shoot.Stop();
        }
    }
}

public struct PointDistanceJob : IJob
{
    [ReadOnly] public PhysicsWorld world;

    public NativeList<DistanceHit> distance_hits;
    public PointDistanceInput point_distance_input;
    public bool collect_all_hits;

    public void Execute()
    {
        if (collect_all_hits)
        {
            world.CalculateDistance(point_distance_input, ref distance_hits);
        }
        else if (world.CalculateDistance(point_distance_input, out DistanceHit hit))
        {
            distance_hits.Add(hit);
        }
    }
}
