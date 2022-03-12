using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ECSManager : MonoBehaviour 
{
    EntityManager manager;

    public GameObject player;

    public GameObject sandPrefab;
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject rockPrefab;
    public GameObject snowPrefab;
    const int worldHalfSize = 75;

    [Range(0.1f, 10.0f)]
    public float strength1 = 1.0f;

    [Range(0.01f, 1.0f)]
    public float scale1 = 0.1f;

    [Range(0.1f, 10.0f)]
    public float strength2 = 1.0f;

    [Range(0.01f, 1.0f)]
    public float scale2 = 0.1f;

    [Range(0.1f, 10.0f)]
    public float strength3 = 1.0f;

    [Range(0.01f, 1.0f)]
    public float scale3 = 0.1f;

    [Range(0.0f, 100.0f)]
    public float sand_altitude = 2f;

    [Range(0.0f, 100.0f)]
    public float dirt_altitude = 4f;

    [Range(0.0f, 100.0f)]
    public float grass_altitude = 6f;

    [Range(0.0f, 100.0f)]
    public float rock_altitude = 8f;

    [Range(0.0f, 100.0f)]
    public float snow_altitude = 10f;

    void Start() {

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        GameDataManager.sand_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(sandPrefab, settings);
        GameDataManager.dirt_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(dirtPrefab, settings);
        GameDataManager.grass_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(grassPrefab, settings);
        GameDataManager.rock_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(rockPrefab, settings);
        GameDataManager.snow_entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(snowPrefab, settings);
        Entity instance;

        for (int z = -worldHalfSize; z <= worldHalfSize; ++z) {

            for (int x = -worldHalfSize; x <= worldHalfSize; ++x) {

                float height = Mathf.PerlinNoise(x * scale1, z * scale1) * strength1;
                var position = new Vector3(x, height, z);

                if (height <= GameDataManager.sand_altitude)
                {
                    instance = manager.Instantiate(GameDataManager.sand_entity);
                }
                else if (height <= GameDataManager.dirt_altitude)
                {
                    instance = manager.Instantiate(GameDataManager.dirt_entity);
                }
                else if (height <= GameDataManager.grass_altitude)
                {
                    instance = manager.Instantiate(GameDataManager.grass_entity);
                }
                else if (height <= GameDataManager.rock_altitude)
                {
                    instance = manager.Instantiate(GameDataManager.rock_entity);
                }
                else
                {
                    instance = manager.Instantiate(GameDataManager.snow_entity);
                }

                manager.SetComponentData(instance, new Translation { Value = position });
                manager.SetComponentData(instance, new BlockData { init_position = position });
            }
        }
    }

    // Update is called once per frame
    void Update() 
    {
        if ((GameDataManager.player_psotion != player.transform.position) ||
            (GameDataManager.strength1 != strength1) ||
            (GameDataManager.scale1 != scale1) ||
            (GameDataManager.strength2 != strength2) ||
            (GameDataManager.scale2 != scale2) ||
            (GameDataManager.strength3 != strength3) ||
            (GameDataManager.scale3 != scale3) ||
            (GameDataManager.sand_altitude != sand_altitude) ||
            (GameDataManager.dirt_altitude != dirt_altitude) ||
            (GameDataManager.grass_altitude != grass_altitude) ||
            (GameDataManager.rock_altitude != rock_altitude) ||
            (GameDataManager.snow_altitude != snow_altitude))
        {
            GameDataManager.changed_flag = true;
        }

        GameDataManager.player_psotion = player.transform.position;

        GameDataManager.strength1 = strength1;
        GameDataManager.scale1 = scale1;
        GameDataManager.strength2 = strength2;
        GameDataManager.scale2 = scale2;
        GameDataManager.strength3 = strength3;
        GameDataManager.scale3 = scale3;

        GameDataManager.sand_altitude = sand_altitude;
        GameDataManager.dirt_altitude = dirt_altitude;
        GameDataManager.grass_altitude = grass_altitude;
        GameDataManager.rock_altitude = rock_altitude;
        GameDataManager.snow_altitude = snow_altitude;
    }
}
