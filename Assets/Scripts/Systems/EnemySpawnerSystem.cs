using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateAfter(typeof(PlaneSpawnerSystem))]
[UpdateAfter(typeof(ObstacleSpawnerSystem))]
public partial struct EnemySpawnerSystem : ISystem
{
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