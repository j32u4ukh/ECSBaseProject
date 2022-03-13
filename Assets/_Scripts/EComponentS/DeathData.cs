using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct DeathData : IComponentData
{
    public bool is_dead;
}
