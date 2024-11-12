using System;
using Game;
using Unity.Entities;

public struct AbilityComponent : IComponentData, IDisposable
{
    public Abilities ability;
    public int level;
    public float cooldown;
    public float cooldownRemaining;
    public int range;
    public float speed;
    public float rotationSpeed;
    public float damage;
    public Entity projectileEntity;
    public float projectileScale;
    public byte hasProjectile;
    public BlobAssetReference<PositionsComponent> positionsToCheck;

    public void Dispose()
    {
        if (positionsToCheck.IsCreated)
        {
            positionsToCheck.Dispose();
        }
    }
}