using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[WithAny(typeof(DestroyComponent))]
public partial struct DestroyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    private void Execute([ChunkIndexInQuery] int sortKey, in Entity entityToDestroy)
    {
        ecb.DestroyEntity(sortKey, entityToDestroy);
    }
}