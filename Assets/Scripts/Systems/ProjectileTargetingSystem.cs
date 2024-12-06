#region

using Components;
using Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

#endregion

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(CollisionDamageSystem))]
    public partial struct ProjectileTargetingSystem : ISystem
    {
        private ComponentLookup<AbilityComponent> abilityComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TargetComponent>();
            state.RequireForUpdate<PlayerAliveComponent>();

            abilityComponentLookup = state.GetComponentLookup<AbilityComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            abilityComponentLookup.Update(ref state);

            ProjectileTargetingJob job = new ProjectileTargetingJob
            {
                deltaTime = SystemAPI.Time.fixedDeltaTime,
                abilityComponentLookup = abilityComponentLookup
            };

            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
            state.Dependency = jobHandle;
        }
    }
}