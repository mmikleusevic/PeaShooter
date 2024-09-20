using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ProjectileDisablingJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly] public ComponentLookup<EnemyComponent> enemyLookup;
    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, ref ProjectileComponent projectile,
        ref LocalTransform transform, in TargetComponent target)
    {
        if (projectile.lifetime <= 0 || projectile.hasCollided || !enemyLookup.HasComponent(target.enemyEntity))
        {
            ecb.SetComponentEnabled<ProjectileComponent>(sortKey, entity, false);
            projectile.hasCollided = false;
            projectile.lifetime = projectile.maxLifetime;

            //Don't want to destroy projectiles so I'll just move them out of sight
            transform.Position = new float3(-500, -500, -500);
        }

        projectile.lifetime -= deltaTime;
    }
}