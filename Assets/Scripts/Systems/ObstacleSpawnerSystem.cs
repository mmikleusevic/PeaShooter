using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
[WithAll(typeof(ObstacleSpawnerComponent))]
public partial struct ObstacleSpawnerSystem : ISystem
{
    private EntityQuery gridEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(ref state);

        state.RequireForUpdate(gridEntityQuery);
        state.RequireForUpdate<ObstacleSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EndInitializationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        GridComponent grid = gridEntityQuery.GetSingleton<GridComponent>();

        ObstacleSpawnJob job = new ObstacleSpawnJob
        {
            ecb = ecb,
            grid = grid,
            seed = (uint)Time.realtimeSinceStartup * 1000
        };

        JobHandle spawnHandle = job.Schedule(state.Dependency);
        state.Dependency = spawnHandle;
    }
}