#region

using Components;
using Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

#endregion

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(UIBarDisableAndPoolSystem))]
    [UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial struct DestroySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<DestroyComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            DestroyJob job = new DestroyJob
            {
                ecb = ecb.AsParallelWriter()
            };

            JobHandle spawnHandle = job.ScheduleParallel(state.Dependency);
            state.Dependency = spawnHandle;
        }
    }
}