using Unity.Entities;

namespace Components
{
    public struct ProjectilesUpdateComponent : IComponentData
    {
        public Entity oldAbilityEntity;
        public Entity newAbilityEntity;
    }
}