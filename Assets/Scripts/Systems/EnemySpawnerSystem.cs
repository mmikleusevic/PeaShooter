using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerSpawnerSystem))]
public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<GridComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GridComponent grid = SystemAPI.GetSingleton<GridComponent>();

        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        EnemySpawnJob job = new EnemySpawnJob
        {
            ecb = ecb,
            grid = grid,
            elapsedTime = SystemAPI.Time.ElapsedTime,
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}