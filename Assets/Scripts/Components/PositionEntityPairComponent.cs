using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct PositionEntityPairComponent : IEquatable<PositionEntityPairComponent>
    {
        public int2 position;
        public Entity enemyEntity;

        public bool Equals(PositionEntityPairComponent other)
        {
            return position.Equals(other.position) && enemyEntity.Equals(other.enemyEntity);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (position.GetHashCode() * 397) ^ enemyEntity.GetHashCode();
            }
        }
    }
}