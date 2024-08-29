using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct ProjectileDisablingJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, ref ProjectileComponent projectile)
    {
        if (projectile.lifetime <= 0 || projectile.hasCollidedWithEnemy)
        {
            ecb.SetComponentEnabled<ProjectileComponent>(sortKey, entity, false);
        }

        projectile.lifetime -= deltaTime;
    }
}