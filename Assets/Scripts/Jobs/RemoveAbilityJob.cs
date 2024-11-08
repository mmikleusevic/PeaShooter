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

        public void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, AbilityComponent ability)
        {
            if (ability.positionsToCheck.IsCreated)
            {
                ability.positionsToCheck.Dispose();
            }

            ecb.DestroyEntity(sortKey, entity);
        }
    }
}