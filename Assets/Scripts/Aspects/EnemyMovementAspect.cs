using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
public readonly partial struct EnemyMovementAspect : IAspect
{
    public readonly RefRO<EnemyComponent> enemyController;
    public readonly RefRO<LocalTransform> transform;
    public readonly RefRW<PhysicsVelocity> physics;
}
