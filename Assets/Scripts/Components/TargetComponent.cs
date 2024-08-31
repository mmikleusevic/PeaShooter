using Unity.Entities;

public struct TargetComponent : IComponentData
{
    public EnemyComponent enemy;
    public Entity enemyEntity;
}
