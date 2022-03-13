using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TriggerData : IComponentData
{
    public float3 trigger_effect;
}
