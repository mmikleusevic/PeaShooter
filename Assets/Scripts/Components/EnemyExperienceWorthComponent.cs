using Unity.Entities;

namespace Components
{
    public struct EnemyExperienceWorthComponent : IComponentData
    {
        public uint experience;
    }
}