using Unity.Entities;

namespace Components
{
    public struct TargetComponent : IComponentData
    {
        public EnemyComponent enemyComponent;
        public Entity enemyEntity;
    }
}