using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(BeginInitializationEntityCommandBufferSystem))]
[WithAll(typeof(GridSpawnerComponent))]
public partial struct GridSpawnerSystem : ISystem
{
    private GridComponent gridComponent;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridSpawnerComponent>();
    }
    public void OnUpdate(ref SystemState state)
    {
        BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        GridSpawnJob job = new GridSpawnJob
        {
            ecb = ecb,
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;

        if (SystemAPI.TryGetSingleton(out GridComponent gridComponent))
        {
            this.gridComponent = gridComponent;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (gridComponent.gridNodes.IsCreated)
        {
            gridComponent.gridNodes.Dispose();
        }
    }
}