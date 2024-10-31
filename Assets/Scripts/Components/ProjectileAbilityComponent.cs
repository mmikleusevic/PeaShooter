using Unity.Entities;

namespace Components
{
    public struct ProjectileAbilityComponent : IComponentData
    {
        public Entity parentEntity;
    }
}