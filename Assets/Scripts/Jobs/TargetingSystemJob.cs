using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct TargetingSystemJob : IJobEntity
{
    [ReadOnly] public float deltaTime;

    private void Execute(in TargetComponent target, in ProjectileComponent projectile, in LocalTransform transform, ref PhysicsVelocity velocity)
    {
        float3 direction = math.normalize(target.enemy.position - transform.Position);

        velocity.Linear = direction * projectile.speed * deltaTime;
    }
}