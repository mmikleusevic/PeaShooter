using Game;
using Unity.Entities;

public struct AbilityComponent : IComponentData
{
    public Abilities ability;
    public int level;
    public float cooldown;
    public float cooldownRemaining;
    public int range;
    public float speed;
    public float damage;
    public Entity projectileEntity;
    public float projectileScale;
    public byte hasProjectile;
    public BlobAssetReference<PositionsBlob> positionsToCheck;
}