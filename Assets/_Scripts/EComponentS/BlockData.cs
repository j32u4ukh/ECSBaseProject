using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BlockData : IComponentData
{
    public float3 init_position;
}
