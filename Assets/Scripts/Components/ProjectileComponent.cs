using Unity.Entities;

public struct ProjectileComponent : IComponentData, IEnableableComponent
{
    public float speed;
    public float damage;
    public float lifetime;
    public float maxLifetime;
    public byte hasCollided;
}
