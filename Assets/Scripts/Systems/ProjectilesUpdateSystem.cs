using Components;
using Jobs;
using Unity.Burst;
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
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectilesUpdateComponent>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            ProjectilesUpdateJob job = new ProjectilesUpdateJob
            {
                ecb = ecb.AsParallelWriter(),
                projectilesUpdateComponent = SystemAPI.GetSingleton<ProjectilesUpdateComponent>()
            };

            JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
            state.Dependency = jobHandle;
        }
    }
}