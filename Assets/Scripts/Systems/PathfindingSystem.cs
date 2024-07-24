using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PathfindingSystem : ISystem
{
    private NativeArray<ObstacleComponent> obstacles;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();
        state.RequireForUpdate<ObstacleComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2 playerPosition = SystemAPI.GetSingleton<PlayerComponent>().position;

        if (obstacles.IsEmpty())
        {
            obstacles = SystemAPI.QueryBuilder()
                .WithAll<ObstacleComponent>()
                .Build()
                .ToComponentDataArray<ObstacleComponent>(Allocator.Persistent);
        }

        PathfindingJob job = new PathfindingJob
        {
            playerPosition = playerPosition,
            obstacles = obstacles
        };

        job.ScheduleParallel();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        obstacles.Dispose();
    }
}