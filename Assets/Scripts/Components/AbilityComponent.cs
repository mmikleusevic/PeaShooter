#region

using System;
using Game;
using Unity.Entities;

#endregion

namespace Components
{
    public struct AbilityComponent : IComponentData, IDisposable
    {
        public Abilities ability;
        public int level;
        public float cooldown;
        public float cooldownRemaining;
        public int range;
        public float speed;
        public float damage;
        public Entity abilityEntity;
        public byte hasProjectile;
        public float scale;
        public BlobAssetReference<PositionsComponent> positionsToCheck;

        public void Dispose()
        {
            if (positionsToCheck.IsCreated)
            {
                positionsToCheck.Dispose();
            }
        }
    }
}