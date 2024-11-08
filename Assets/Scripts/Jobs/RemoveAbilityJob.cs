using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
    [BurstCompile]
    [WithAll(typeof(RemoveAbilityComponent))]
    public partial struct RemoveAbilityJob : IJobEntity
    {
        public EntityCommandBuffer ecb;

        public void Execute(in Entity entity, ref AbilityComponent ability)
        {
            ability.Dispose();

            ecb.DestroyEntity(entity);
        }
    }
}