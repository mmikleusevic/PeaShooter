using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateAfter(typeof(PlaneSpawnerSystem))]
public partial struct ObstacleSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (randomData, spawner) in SystemAPI.Query<RandomDataComponent, ObstacleSpawnerComponent>())
        {
            ObstacleSpawnJob job = new ObstacleSpawnJob
            {
                commandBuffer = ecb,
                randomData = randomData,
                prefabToSpawn = spawner.prefab,
            };

            JobHandle jobHandle = job.Schedule(spawner.numberToSpawn, state.Dependency);

            jobHandle.Complete();
        }
    }
}
