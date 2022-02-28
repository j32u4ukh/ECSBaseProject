using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using UnityEngine.UI;

namespace udemy
{
    public class ECSInterface : MonoBehaviour
    {
        public GameObject sheep_prefab;
        public Button count_sheep_button;
        public Text sheep_number_text;

        public GameObject tank_prefab;
        public Button count_tank_button;
        public Text tank_number_text;

        public GameObject plam_tree_prefab;
        public Button count_plam_tree_button;
        public Text plam_tree_number_text;

        World world;
        EntityManager manager;

        // Start is called before the first frame update
        void Start()
        {
            world = World.DefaultGameObjectInjectionWorld;
            manager = world.GetExistingSystem<MoveSystem>().EntityManager;
            Debug.Log($"All entites: {manager.GetAllEntities().Length}");

            count_sheep_button.onClick.AddListener(countSheep);
            count_tank_button.onClick.AddListener(countTank);
            count_plam_tree_button.onClick.AddListener(countPlamTree);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Instantiate(sheep_prefab, 
                            new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)), 
                            Quaternion.identity,
                            parent: transform);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Instantiate(tank_prefab,
                            new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)),
                            Quaternion.identity,
                            parent: transform);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                GameObjectConversionSettings setting = GameObjectConversionSettings.FromWorld(world, null);
                Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(plam_tree_prefab, setting);

                Entity instance = manager.Instantiate(prefab);
                Vector3 position = transform.TransformPoint(new float3(UnityEngine.Random.Range(-10, 10),
                                                                       0,
                                                                       UnityEngine.Random.Range(-10, 10)));
                manager.SetComponentData(instance, new Translation { Value = position });
                manager.SetComponentData(instance, new Rotation { Value = new quaternion(0, 0, 0, 0) });
            }
        }

        void countSheep()
        {
            EntityQuery entityQuery = manager.CreateEntityQuery(ComponentType.ReadOnly<SheepData>());
            //Debug.Log($"#sheep entites: {entityQuery.CalculateEntityCount()}");
            sheep_number_text.text = $"#sheep entites: {entityQuery.CalculateEntityCount()}";
        }

        void countTank()
        {
            EntityQuery entityQuery = manager.CreateEntityQuery(ComponentType.ReadOnly<TankData>());
            //Debug.Log($"#sheep entites: {entityQuery.CalculateEntityCount()}");
            tank_number_text.text = $"#tank entites: {entityQuery.CalculateEntityCount()}";
        }

        void countPlamTree()
        {
            EntityQuery entityQuery = manager.CreateEntityQuery(ComponentType.ReadOnly<PlamTreeData>());
            //Debug.Log($"#sheep entites: {entityQuery.CalculateEntityCount()}");
            tank_number_text.text = $"#plam tree entites: {entityQuery.CalculateEntityCount()}";
        }
    }

}