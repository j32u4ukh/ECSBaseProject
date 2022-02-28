using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace udemy
{
    public class ESCManager : MonoBehaviour
    {
        EntityManager manager;
        public GameObject sheepPrefab;
        const int numSheep = 15000;

        // Start is called before the first frame update
        void Start()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            GameObjectConversionSettings setting = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(sheepPrefab, setting);

            for(int i = 0; i < numSheep; i++)
            {
                Entity instance = manager.Instantiate(prefab);
                Vector3 position = transform.TransformPoint(new float3(UnityEngine.Random.Range(-50,  50),
                                                                       UnityEngine.Random.Range(  0, 100),
                                                                       UnityEngine.Random.Range(-50,  50)));
                manager.SetComponentData(instance, new Translation { Value = position});
                manager.SetComponentData(instance, new Rotation { Value = new quaternion(0, 0, 0 ,0) });

            }
        }
    }
}
