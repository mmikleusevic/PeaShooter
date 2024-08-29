using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerSpawnerSystem))]
public partial struct EnemySpawnerSystem : ISystem
{
    private BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<GridComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GridComponent grid = SystemAPI.GetSingleton<GridComponent>();

        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        EnemySpawnJob job = new EnemySpawnJob
        {
            ecb = ecb,
            elapsedTime = SystemAPI.Time.ElapsedTime,
            grid = grid
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}