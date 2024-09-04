using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(GridSpawnerSystem))]
public partial struct ObstacleSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleSpawnerComponent>();
        state.RequireForUpdate<GridComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        GridComponent grid = SystemAPI.GetSingleton<GridComponent>();

        ObstacleSpawnJob spawnJob = new ObstacleSpawnJob
        {
            ecb = ecb,
            grid = grid
        };

        JobHandle spawnHandle = spawnJob.Schedule(state.Dependency);
        spawnHandle.Complete();
    }
}