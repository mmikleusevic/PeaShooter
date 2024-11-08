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
    [UpdateAfter(typeof(AbilitySystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct UpdateProjectilesSystem : ISystem
    {
        private EntityQuery projectileUpdateQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            projectileUpdateQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UpdateProjectilesComponent>()
                .Build(ref state);

            projectileUpdateQuery.SetChangedVersionFilter(ComponentType.ReadOnly<UpdateProjectilesComponent>());

            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(projectileUpdateQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EndInitializationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            UpdateProjectilesJob job = new UpdateProjectilesJob
            {
                updateProjectiles = projectileUpdateQuery.GetSingleton<UpdateProjectilesComponent>()
            };

            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);

            jobHandle.Complete();

            ecb.DestroyEntity(projectileUpdateQuery, EntityQueryCaptureMode.AtPlayback);

            state.Dependency = jobHandle;
        }
    }
}