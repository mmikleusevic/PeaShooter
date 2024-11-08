using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(UpdateProjectilesSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct RemoveAbilitySystem : ISystem
    {
        private EntityQuery removeAbilityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            removeAbilityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RemoveAbilityComponent, AbilityComponent>()
                .Build(ref state);

            removeAbilityQuery.SetChangedVersionFilter(ComponentType.ReadOnly<RemoveAbilityComponent>());

            state.RequireForUpdate(removeAbilityQuery);
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            RemoveAbilityJob job = new RemoveAbilityJob
            {
                ecb = ecb
            };

            JobHandle jobHandle = job.Schedule(state.Dependency);
            state.Dependency = jobHandle;
        }
    }
}