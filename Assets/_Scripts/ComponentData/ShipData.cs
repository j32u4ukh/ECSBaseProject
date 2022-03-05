using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShipData : IComponentData
{
    public float speed;
    public float rotationSpeed;
    public int current_wp;
    public bool approach;

    public Entity bullet_entity;
    public Entity explosion_entity;
}
