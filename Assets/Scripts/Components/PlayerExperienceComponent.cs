using Unity.Entities;

namespace Components
{
    public struct PlayerExperienceComponent : IComponentData
    {
        public uint points;
        public int currentLevel;
    }
}