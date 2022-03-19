using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    EntityManager manager;
    BlobAssetStore store;

    public GameObject zombie_prefab;
    public Transform[] towers;

    const int n_zombie = 500;

    // Start is called before the first frame update
    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        store = new BlobAssetStore();

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        Entity zombie_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(zombie_prefab, settings);
        Entity entity;
        float x, z;
        int n_tower = towers.Length;
        DataManager.towers = new float3[n_tower];

        for (int i = 0; i < n_tower; i++)
        {
            DataManager.towers[i] = towers[i].position;
        }

        for (int i = 0; i < n_zombie; i++)
        {
            entity = manager.Instantiate(zombie_entity);
            x = UnityEngine.Random.Range(-200, 200);
            z = UnityEngine.Random.Range(-200, 200);

            manager.SetComponentData(entity, new Translation { Value = new float3(x, 1.8f, z)});
            manager.SetComponentData(entity, new ZombieData { 
                target = UnityEngine.Random.Range(0, n_tower),
                speed = 150f,
                rotate_speed = 10f
            });
        }
    }

    private void OnDestroy()
    {
        store.Dispose();
    }
}
