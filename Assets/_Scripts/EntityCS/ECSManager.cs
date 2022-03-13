using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ECSManager : MonoBehaviour 
{
    public EntityTracker tracker;

    EntityManager manager;
    public GameObject player_prefab;
    public GameObject bullet_prefab;
    BlobAssetStore store;

    void Start() {

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        store = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        Entity player_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(player_prefab, settings);
        Entity bullet_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(bullet_prefab, settings);

        GameDataManager.bullet_points = new float3[2];
        GameDataManager.bullet_points[0] = new float3(0.4f, 0.6f, 0.9f);
        GameDataManager.bullet_points[1] = new float3(-0.4f, 0.6f, 0.9f);

        Entity player = manager.Instantiate(player_entity);
        manager.SetComponentData(player, new Translation { Value = new float3(0f, 2.55f, 0f) });
        manager.SetComponentData(player, new CharacterData { 
            speed = 1f,
            bullet_entity = bullet_entity
        });

        tracker.setReceivedEntity(tracking_target: player);
    }

    private void OnDestroy()
    {
        store.Dispose();
    }
}
