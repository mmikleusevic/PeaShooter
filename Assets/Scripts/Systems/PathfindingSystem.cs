using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct PathfindingSystem : ISystem, ISystemStartStop
{
    private NativeList<ObstacleComponent> obstacles;
    private Entity obstacleBufferEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.Enabled = false;

        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();
        state.RequireForUpdate<ObstacleComponent>();
        state.RequireForUpdate<ObstacleListComponent>();
    }

    public void OnStartRunning(ref SystemState state)
    {
        obstacleBufferEntity = SystemAPI.GetSingletonEntity<ObstacleListComponent>();
        obstacles = SystemAPI.GetSingleton<ObstacleListComponent>().obstacles;
    }

    public void OnStopRunning(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2 playerPosition = SystemAPI.GetSingleton<PlayerComponent>().position;

        PathfindingJob job = new PathfindingJob
        {
            playerPosition = playerPosition,
            obstacles = obstacles,
            obstacleBufferEntity = obstacleBufferEntity
        };

        JobHandle handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}