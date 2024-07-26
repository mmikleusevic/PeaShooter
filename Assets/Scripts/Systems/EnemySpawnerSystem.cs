using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerSpawnerSystem))]
public partial struct EnemySpawnerSystem : ISystem
{
    private NativeList<ObstacleComponent> obstacles;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<ObstacleComponent>();
        state.RequireForUpdate<ObstacleListComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (obstacles.IsEmpty)
        {
            ObstacleListComponent obstaclesList = SystemAPI.GetSingleton<ObstacleListComponent>();

            obstacles = obstaclesList.obstacles;
        }

        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        EnemySpawnJob job = new EnemySpawnJob
        {
            ecb = ecb,
            obstacles = obstacles,
            elapsedTime = SystemAPI.Time.ElapsedTime,
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}