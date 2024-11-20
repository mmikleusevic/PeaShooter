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

        private void Execute(ref AbilityComponent abilityComponent, in Entity abilityEntity)
        {
            abilityComponent.Dispose();

            ecb.AddComponent<DestroyComponent>(abilityEntity);
        }
    }
}