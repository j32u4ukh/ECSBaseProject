using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ZombieData : IComponentData
{
    public int target;
    public float speed;
    public float rotate_speed;
}