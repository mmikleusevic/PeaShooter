#region

using Unity.Entities;

#endregion

namespace Components
{
    public struct TargetComponent : IComponentData
    {
        public EnemyComponent enemyComponent;
        public Entity enemyEntity;
    }
}