using Unity.Entities;

namespace Components
{
    public struct UpdateProjectilesComponent : IComponentData
    {
        public Entity oldAbilityEntity;
        public Entity newAbilityEntity;
    }
}