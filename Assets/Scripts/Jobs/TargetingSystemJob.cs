using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct TargetingSystemJob : IJobEntity
{
    [ReadOnly] public float deltaTime;

    private void Execute(in TargetComponent target, in ProjectileComponent projectile, ref LocalTransform transform)
    {
        float3 direction = math.normalize(target.enemy.position - transform.Position);
        float3 newPosition = transform.Position + direction;
        float factor = projectile.speed * deltaTime;

        transform.Position = math.lerp(transform.Position, newPosition, factor);
    }
}