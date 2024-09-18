using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(PlayerSpawnerSystem))]
public partial struct EnemySpawnerSystem : ISystem
{
    private EntityQuery gridEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(ref state);

        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate(gridEntityQuery);
        state.RequireForUpdate<EnemySpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GridComponent grid = gridEntityQuery.GetSingleton<GridComponent>();

        BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        uint seed = math.hash(new int2(Time.frameCount, (int)(SystemAPI.Time.ElapsedTime * 1000)));

        EnemySpawnJob job = new EnemySpawnJob
        {
            ecb = ecb.AsParallelWriter(),
            elapsedTime = SystemAPI.Time.ElapsedTime,
            grid = grid,
            seed = seed
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}