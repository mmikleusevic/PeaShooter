using Unity.Entities;

public struct AbilityComponent : IComponentData
{
    public float cooldown;
    public float cooldownRemaining;
    public float range;
    public Entity projectileEntity;
    public byte hasProjectile;
    public float projectileScale;
}