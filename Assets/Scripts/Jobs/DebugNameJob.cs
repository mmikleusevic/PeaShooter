using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
    [BurstCompile]
    public partial struct DebugNameJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        private void Execute([ChunkIndexInQuery] int sortKey, in DebugNameComponent debugNameComponentRO,
            in Entity debugNameEntity)
        {
            ecb.SetName(sortKey, debugNameEntity, debugNameComponentRO.entityName);
            ecb.RemoveComponent<DebugNameComponent>(sortKey, debugNameEntity);
        }
    }
}