using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
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

        uint seed = math.hash(new int2(Time.frameCount, (int)(SystemAPI.Time.ElapsedTime * 1000)));

        ObstacleSpawnJob job = new ObstacleSpawnJob
        {
            ecb = ecb,
            grid = grid,
            seed = seed
        };

        JobHandle spawnHandle = job.Schedule(state.Dependency);
        state.Dependency = spawnHandle;
    }
}