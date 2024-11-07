using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [BurstCompile]
    public struct PositionEntityPairComponent : IEquatable<PositionEntityPairComponent>
    {
        public int2 position;
        public Entity entity;

        public bool Equals(PositionEntityPairComponent other)
        {
            return position.Equals(other.position) && entity.Equals(other.entity);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (position.GetHashCode() * 397) ^ entity.GetHashCode();
            }
        }
    }
}