using Unity.Entities;

public struct TargetComponent : IComponentData
{
    public EnemyComponent enemyComponent;
    public Entity enemyEntity;
}