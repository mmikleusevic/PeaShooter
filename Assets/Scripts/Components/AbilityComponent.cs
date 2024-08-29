using Unity.Entities;

public struct AbilityComponent : IComponentData
{
    public float cooldown;
    public float cooldownRemaining;
    public int damage;
    public float range;
    public Entity projectileEntity;
}