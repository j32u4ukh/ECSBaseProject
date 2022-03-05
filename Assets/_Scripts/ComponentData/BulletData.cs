using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BulletData : IComponentData
{
    public int target;
    //public float speed;
    //public float3 destination;
}
