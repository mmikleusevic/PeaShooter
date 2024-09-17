using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial struct GridSpawnerSystem : ISystem
{
    private EntityQuery gridEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(ref state);

        state.RequireForUpdate<GridSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        GridSpawnJob job = new GridSpawnJob
        {
            ecb = ecb
        };

        JobHandle jobHandle = job.Schedule(state.Dependency);
        state.Dependency = jobHandle;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (gridEntityQuery.HasSingleton<GridComponent>())
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();
            gridComponent.gridNodes.Dispose();

            state.EntityManager.DestroyEntity(gridEntityQuery);
        }
    }
}