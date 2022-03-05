﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ECSManager : MonoBehaviour
{
    EntityManager manager;
    public GameObject prefab_obj;
    const int numTanks = 500;

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.prefab_obj, settings);
        float3[] wps = GameDataManager.instance.wps;

        for (int i = 0; i < numTanks; i++)
        {
            var instance = manager.Instantiate(prefab);
            float x = UnityEngine.Random.Range(-300, 300);
            float y = UnityEngine.Random.Range(-300, 300);
            float z = UnityEngine.Random.Range(-300, 300);
            var position = transform.TransformPoint(new float3(x, y, z));
            manager.SetComponentData(instance, new Translation { Value = position });

            var q = Quaternion.Euler(new Vector3(0, 45, 0));
            manager.SetComponentData(instance, new Rotation { Value = q });

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

            manager.SetComponentData(instance, new ShipData { 
                speed = UnityEngine.Random.Range(5.0f, 20.0f),
                rotationSpeed = UnityEngine.Random.Range(3.0f, 5.0f),
                current_wp = closest_wp,
            });
        }

    }
}
