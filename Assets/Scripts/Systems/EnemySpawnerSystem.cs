using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
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
    }

    public void OnUpdate(ref SystemState state)
    {
        GridComponent grid = gridEntityQuery.GetSingleton<GridComponent>();

        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        EnemySpawnJob job = new EnemySpawnJob
        {
            ecb = ecb.AsParallelWriter(),
            elapsedTime = SystemAPI.Time.ElapsedTime,
            grid = grid
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}