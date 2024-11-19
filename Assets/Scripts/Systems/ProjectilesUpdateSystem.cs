using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(BeginInitializationEntityCommandBufferSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct ProjectilesUpdateSystem : ISystem
    {
        private EntityQuery projectileUpdateQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            projectileUpdateQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ProjectilesUpdateComponent>()
                .Build(ref state);

            projectileUpdateQuery.SetChangedVersionFilter(ComponentType.ReadOnly<ProjectilesUpdateComponent>());

            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(projectileUpdateQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            ProjectilesUpdateJob job = new ProjectilesUpdateJob
            {
                ProjectilesUpdate = projectileUpdateQuery.GetSingleton<ProjectilesUpdateComponent>()
            };

            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);

            ecb.DestroyEntity(projectileUpdateQuery, EntityQueryCaptureMode.AtPlayback);

            state.Dependency = jobHandle;
        }
    }
}