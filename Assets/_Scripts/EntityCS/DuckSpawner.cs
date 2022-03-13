using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class DuckSpawner : MonoBehaviour
{
    EntityManager manager;
    BlobAssetStore store;

    public GameObject bullet_prefab;
    public Transform[] bullet_points;

    public GameObject duck_prefab;
    const int n_duck = 5000;

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        store = new BlobAssetStore();

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        Entity duck_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(duck_prefab, settings);

        for (int i = 0; i < n_duck; i++)
        {
            var instance = manager.Instantiate(duck_entity);
            float x = UnityEngine.Random.Range(-200, 200);
            float y = UnityEngine.Random.Range(  50, 200);
            float z = UnityEngine.Random.Range(-200, 200);

            manager.SetComponentData(instance, new Translation { Value = new float3(x, y, z) });
            manager.SetComponentData(instance, new DeathData { is_dead = false });
        }
    }

    private void OnDestroy()
    {
        store.Dispose();
    }
}
