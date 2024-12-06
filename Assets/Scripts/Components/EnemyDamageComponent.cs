#region

using Unity.Entities;

#endregion

namespace Components
{
    public struct EnemyDamageComponent : IComponentData
    {
        public float damagePerSecond;
    }
}