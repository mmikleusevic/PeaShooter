#region

using Unity.Entities;

#endregion

namespace Components
{
    public struct EnemyExperienceWorthComponent : IComponentData
    {
        public uint experience;
    }
}