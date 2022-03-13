using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CharacterData : IComponentData
{
    public float speed;
    public Entity bullet_entity;
}
