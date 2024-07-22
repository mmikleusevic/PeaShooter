using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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

        foreach (var (randomData, spawner, transform) in SystemAPI.Query<RandomDataComponent, RefRO<ObstacleSpawnerComponent>, RefRO<LocalTransform>>())
        {
            ObstacleSpawnJob job = new ObstacleSpawnJob
            {
                commandBuffer = ecb,
                positionsOccupied = new NativeList<float3>(Allocator.TempJob),
                randomData = randomData,
                distance = transform.ValueRO.Scale,
                prefabToSpawn = spawner.ValueRO.prefab,
            };

            JobHandle jobHandle = job.Schedule(spawner.ValueRO.numberToSpawn, state.Dependency);

            jobHandle.Complete();

            job.positionsOccupied.Dispose();
        }
    }
}
