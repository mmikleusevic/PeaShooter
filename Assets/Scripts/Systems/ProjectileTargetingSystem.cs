using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(CollisionDamageSystem))]
    public partial struct ProjectileTargetingSystem : ISystem
    {
        private ComponentLookup<AbilityComponent> abilityLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TargetComponent>();
            state.RequireForUpdate<PlayerAliveComponent>();

            abilityLookup = state.GetComponentLookup<AbilityComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            abilityLookup.Update(ref state);

            ProjectileTargetingJob job = new ProjectileTargetingJob
            {
                deltaTime = SystemAPI.Time.fixedDeltaTime,
                AbilityLookup = abilityLookup
            };

            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
            state.Dependency = jobHandle;
        }
    }
}