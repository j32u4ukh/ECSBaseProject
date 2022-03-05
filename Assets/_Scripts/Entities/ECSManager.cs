using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ECSManager : MonoBehaviour
{
    EntityManager manager;
    public GameObject ship_prefab;
    public GameObject bullet_prefab;
    public GameObject explosion_prefab;
    const int numTanks = 100;

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var ship_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(ship_prefab, settings);
        var bullet_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(bullet_prefab, settings);
        var explosion_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(explosion_prefab, settings);
        float3[] wps = GameDataManager.instance.wps;

        //List<GameObject> bullet_spawn_points = new List<GameObject>();
        List<float3> bullet_spawn_points = new List<float3>();
        Transform ship_transform = ship_prefab.transform;

        foreach(Transform part in ship_transform)
        {
            if (part.tag.Equals("BulletSpawnPoint"))
            {
                // 因上層模型有被縮放，導致下層的相對位置需要透過 TransformPoint 來取得世界座標
                bullet_spawn_points.Add(part.TransformPoint(part.position));
            }
        }

        GameDataManager.instance.gun_locations = bullet_spawn_points.ToArray();

        //int n_gun = bullet_spawn_points.Count;
        //GameDataManager.instance.gun_locations = new float3[n_gun];

        //for(int i = 0; i < n_gun; i++)
        //{
        //    GameDataManager.instance.gun_locations[i] = bullet_spawn_points[i].transform.position;
        //}

        for (int i = 0; i < numTanks; i++)
        {
            var ship = manager.Instantiate(ship_entity);
            float x = UnityEngine.Random.Range(-300, 300);
            float y = UnityEngine.Random.Range(-300, 300);
            float z = UnityEngine.Random.Range(-300, 300);
            var position = transform.TransformPoint(new float3(x, y, z));
            manager.SetComponentData(ship, new Translation { Value = position });

            var q = Quaternion.Euler(new Vector3(0, 45, 0));
            manager.SetComponentData(ship, new Rotation { Value = q });

            int closest_wp = 0, n_wp = wps.Length;
            float distance, closest_distance = Mathf.Infinity;

            for(int w = 0; w < n_wp; w++)
            {
                distance = math.distance(position, wps[w]);

                if(distance < closest_distance)
                {
                    closest_distance = distance;
                    closest_wp = w;
                }
            }

            manager.SetComponentData(ship, new ShipData { 
                speed = UnityEngine.Random.Range(5.0f, 20.0f),
                rotationSpeed = UnityEngine.Random.Range(3.0f, 5.0f),
                current_wp = closest_wp,
                bullet_entity = bullet_entity,
                explosion_entity = explosion_entity
            });
        }

    }
}
