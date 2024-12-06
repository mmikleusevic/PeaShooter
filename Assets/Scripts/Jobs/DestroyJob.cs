using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
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
}