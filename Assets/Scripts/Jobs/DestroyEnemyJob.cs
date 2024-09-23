using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct DestroyEnemyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    private void Execute([ChunkIndexInQuery] int sortKey, in EnemyDeadComponent enemyDeadComponent, Entity entity)
    {
        ecb.DestroyEntity(sortKey, entity);
    }
}