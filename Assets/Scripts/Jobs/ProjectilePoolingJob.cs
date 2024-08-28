using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct ProjectilePoolingJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, Entity entity, ref ProjectileComponent projectile)
    {
        projectile.lifetime -= deltaTime;

        if (projectile.lifetime <= 0 || projectile.hasCollidedWithEnemy)
        {
            ecb.SetComponentEnabled<ProjectileComponent>(sortKey, entity, false);
        }
    }
}