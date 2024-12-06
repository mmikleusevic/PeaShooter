using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct HealthComponent : IComponentData
    {
        public float maxHitPoints;

        private float hitPoints;

        public float HitPoints
        {
            readonly get => hitPoints;
            set
            {
                hitPoints = math.clamp(value, 0f, maxHitPoints);

                if (hitPoints == 0) IsDead = true;
            }
        }

        public bool IsDead { get; private set; }
    }
}