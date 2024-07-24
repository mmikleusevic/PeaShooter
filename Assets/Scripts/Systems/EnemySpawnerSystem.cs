using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
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

        job.Schedule();
    }
}