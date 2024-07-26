using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial struct ObstacleSpawnerSystem : ISystem
{
    private SystemHandle pathfindingSystemHandle;
    private NativeList<ObstacleComponent> obstacles;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleSpawnerComponent>();

        pathfindingSystemHandle = state.World.GetOrCreateSystem<PathfindingSystem>();
        obstacles = new NativeList<ObstacleComponent>(Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        NativeList<float3> positionsOccupied = new NativeList<float3>(Allocator.TempJob);

        ObstacleSpawnJob spawnJob = new ObstacleSpawnJob
        {
            ecb = ecb,
            positionsOccupied = positionsOccupied
        };

        JobHandle spawnHandle = spawnJob.Schedule(state.Dependency);
        spawnHandle.Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        ObstacleUpdateJob obstacleJob = new ObstacleUpdateJob
        {
            obstacles = obstacles
        };

        JobHandle obstacleHandle = obstacleJob.Schedule(state.Dependency);
        state.Dependency = obstacleHandle;

        Entity obstacleListEntity = state.EntityManager.CreateEntity();

        state.EntityManager.AddComponentData(obstacleListEntity, new ObstacleListComponent
        {
            obstacles = obstacles
        });

        positionsOccupied.Dispose();

        state.WorldUnmanaged.ResolveSystemStateRef(pathfindingSystemHandle).Enabled = true;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        obstacles.Dispose();
    }
}