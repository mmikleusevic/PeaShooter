using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
public readonly partial struct EnemyMovementAspect : IAspect
{
    public readonly RefRO<EnemyComponent> enemyController;
    public readonly RefRW<LocalTransform> transform;
    public readonly RefRW<PhysicsVelocity> physics;
}
