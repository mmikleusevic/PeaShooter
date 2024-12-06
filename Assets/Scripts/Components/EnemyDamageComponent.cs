using Unity.Entities;

namespace Components
{
    public struct EnemyDamageComponent : IComponentData
    {
        public float damagePerSecond;
    }
}