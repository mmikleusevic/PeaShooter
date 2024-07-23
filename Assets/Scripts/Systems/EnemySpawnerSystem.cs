using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<ObstacleComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        EnemySpawnJob job = new EnemySpawnJob
        {
            ecb = ecb,
            elapsedTime = SystemAPI.Time.ElapsedTime,
        };

        JobHandle jobHandle = job.Schedule(state.Dependency);
        state.Dependency = jobHandle;
    }
}