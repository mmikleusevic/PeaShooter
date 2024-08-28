using Unity.Entities;

public struct ProjectileComponent : IComponentData, IEnableableComponent
{
    public float speed;
    public float lifetime;
    public bool hasCollidedWithEnemy;
}
