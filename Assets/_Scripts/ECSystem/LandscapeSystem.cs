using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Rendering;

public class LandscapeSystem : JobComponentSystem
{
    EntityQuery block_query;

    protected override void OnCreate()
    {
        block_query = GetEntityQuery(typeof(BlockData));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float strength1 = GameDataManager.strength1;
        float scale1 = GameDataManager.scale1;
        float strength2 = GameDataManager.strength2;
        float scale2 = GameDataManager.scale2;
        float strength3 = GameDataManager.strength3;
        float scale3 = GameDataManager.scale3;

        float3 offset = GameDataManager.player_psotion;

        var handle = Entities.WithName("LandscapeSystem")
                             .ForEach((ref Translation positon, ref BlockData block_data) =>
                             {
                                 float3 vertex = block_data.init_position + offset;
                                 float perlin1 = Mathf.PerlinNoise(vertex.x * scale1, vertex.z * scale1) * strength1;
                                 float perlin2 = Mathf.PerlinNoise(vertex.x * scale2, vertex.z * scale2) * strength2;
                                 float perlin3 = Mathf.PerlinNoise(vertex.x * scale3, vertex.z * scale3) * strength3;
                                 float height = perlin1 + perlin2 + perlin3;
                                 positon.Value = new float3(vertex.x, height, vertex.z);
                             })
                             .Schedule(inputDeps);
        handle.Complete();

        if (GameDataManager.changed_flag)
        {
            using (var blocks = block_query.ToEntityArray(Allocator.TempJob))
            {
                float height;
                RenderMesh block_render_mesh;

                Material sand_material = EntityManager.GetSharedComponentData<RenderMesh>(GameDataManager.sand_entity).material;
                Material dirt_material = EntityManager.GetSharedComponentData<RenderMesh>(GameDataManager.dirt_entity).material;
                Material grass_material = EntityManager.GetSharedComponentData<RenderMesh>(GameDataManager.grass_entity).material;
                Material rock_material = EntityManager.GetSharedComponentData<RenderMesh>(GameDataManager.rock_entity).material;
                Material snow_material = EntityManager.GetSharedComponentData<RenderMesh>(GameDataManager.snow_entity).material;

                foreach (var block in blocks)
                {
                    block_render_mesh = EntityManager.GetSharedComponentData<RenderMesh>(block);
                    height = EntityManager.GetComponentData<Translation>(block).Value.y;

                    if (height <= GameDataManager.sand_altitude)
                    {
                        block_render_mesh.material = sand_material;
                    }
                    else if (height <= GameDataManager.dirt_altitude)
                    {
                        block_render_mesh.material = dirt_material;
                    }
                    else if (height <= GameDataManager.grass_altitude)
                    {
                        block_render_mesh.material = grass_material;
                    }
                    else if (height <= GameDataManager.rock_altitude)
                    {
                        block_render_mesh.material = rock_material;
                    }
                    else
                    {
                        block_render_mesh.material = snow_material;
                    }

                    EntityManager.SetSharedComponentData(block, block_render_mesh);
                }
            }

            GameDataManager.changed_flag = false;
        }

        return inputDeps;
    }
}
