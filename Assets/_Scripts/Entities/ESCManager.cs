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
        public GameObject prefab_obj;
        const int number = 5000;

        // Start is called before the first frame update
        void Start()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            GameObjectConversionSettings setting = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab_obj, setting);

            for(int i = 0; i < number; i++)
            {
                Entity instance = manager.Instantiate(prefab);
                float x = Mathf.Sin(i) * UnityEngine.Random.Range(15.0f, 30.0f);
                float y = UnityEngine.Random.Range(-2.0f, 2.0f);
                float z = Mathf.Cos(i) * UnityEngine.Random.Range(15.0f, 30.0f);
                Vector3 position = transform.TransformPoint(new float3(x, y, z));
                manager.SetComponentData(instance, new Translation { Value = position});

                quaternion q = Quaternion.Euler(0f, 0f, 0f);
                manager.SetComponentData(instance, new Rotation { Value = q });

                // y 方向較扁的形狀
                float3 shape = new float3(UnityEngine.Random.Range(5.0f, 15.0f),
                                          UnityEngine.Random.Range(5.0f, 10.0f),
                                          UnityEngine.Random.Range(5.0f, 15.0f));

                float3 scale = shape * UnityEngine.Random.Range(8.0f, 10.0f);

                manager.AddComponent(instance, ComponentType.ReadWrite<NonUniformScale>());
                manager.SetComponentData(instance, new NonUniformScale { Value = scale });
            }
        }
    }
}
