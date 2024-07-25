using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct PathfindingSystem : ISystem, ISystemStartStop
{
    private BufferLookup<ObstacleBuffer> obstacleLookup;
    private Entity obstacleBufferEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.Enabled = false;

        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();
        state.RequireForUpdate<ObstacleComponent>();
        state.RequireForUpdate<ObstacleBuffer>();
    }

    public void OnStartRunning(ref SystemState state)
    {
        obstacleBufferEntity = SystemAPI.GetSingletonEntity<ObstacleBuffer>();
        obstacleLookup = state.GetBufferLookup<ObstacleBuffer>(true);
    }

    public void OnStopRunning(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2 playerPosition = SystemAPI.GetSingleton<PlayerComponent>().position;

        //PathfindingJob job = new PathfindingJob
        //{
        //    playerPosition = playerPosition,
        //    obstacleLookup = obstacleLookup,
        //    obstacleBufferEntity = obstacleBufferEntity
        //};

        //job.ScheduleParallel();
    }
}