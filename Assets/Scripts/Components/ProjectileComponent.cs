using Unity.Entities;

namespace Components
{
    public struct ProjectileComponent : IComponentData, IEnableableComponent
    {
        public float lifetime;
        public float maxLifetime;
        public byte hasCollided;
    }
}