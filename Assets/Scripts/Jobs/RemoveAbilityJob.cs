using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
    [BurstCompile]
    [WithAll(typeof(RemoveAbilityComponent))]
    public partial struct RemoveAbilityJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute([ChunkIndexInQuery] int sortKey, in Entity entity)
        {
            ecb.DestroyEntity(sortKey, entity);
        }
    }
}