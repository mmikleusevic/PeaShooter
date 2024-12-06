#region

using Unity.Entities;

#endregion

namespace Components
{
    public struct PlayerExperienceComponent : IComponentData
    {
        public uint points;
        public int currentLevel;
    }
}