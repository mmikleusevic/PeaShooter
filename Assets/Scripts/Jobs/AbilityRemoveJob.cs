using Components;
using Unity.Burst;
using Unity.Entities;

namespace Jobs
{
    [BurstCompile]
    [WithAll(typeof(AbilityRemoveComponent))]
    public partial struct AbilityRemoveJob : IJobEntity
    {
        public EntityCommandBuffer ecb;

        public void Execute(in Entity entity, ref AbilityComponent ability)
        {
            ability.Dispose();

            ecb.AddComponent<DestroyComponent>(entity);
        }
    }
}