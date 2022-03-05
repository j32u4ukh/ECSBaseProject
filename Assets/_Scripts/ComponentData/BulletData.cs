using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BulletData : IComponentData
{
    public int target;
    public Entity explosion_entity;
    //public float speed;
    //public float3 destination;
}
