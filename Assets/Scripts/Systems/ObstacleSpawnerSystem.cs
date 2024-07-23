using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
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
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        NativeList<float3> positionsOccupied = new NativeList<float3>(Allocator.TempJob);

        foreach (var (randomData, spawner, transform) in SystemAPI.Query<RandomDataComponent, RefRO<ObstacleSpawnerComponent>, RefRO<LocalTransform>>())
        {
            ObstacleSpawnJob job = new ObstacleSpawnJob
            {
                ecb = ecb,
                positionsOccupied = positionsOccupied,
                randomData = randomData,
                distance = transform.ValueRO.Scale,
                prefabToSpawn = spawner.ValueRO.prefab,
            };

            JobHandle spawnJobHandle = job.Schedule(spawner.ValueRO.numberToSpawn, state.Dependency);
            spawnJobHandle.Complete();

            state.Dependency = spawnJobHandle;
        }

        positionsOccupied.Dispose();
    }
}
