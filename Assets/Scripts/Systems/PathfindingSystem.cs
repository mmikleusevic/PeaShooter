using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct PathfindingSystem : ISystem
{
    private EntityQuery playerEntityQuery;
    private EntityQuery gridEntityQuery;
    private float timer;
    private float targetTime;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerComponent>()
            .Build(ref state);

        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(ref state);

        timer = 0.5f;
        targetTime = timer;

        state.RequireForUpdate(playerEntityQuery);
        state.RequireForUpdate(gridEntityQuery);
        state.RequireForUpdate<EnemyComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        timer += SystemAPI.Time.DeltaTime;

        if (timer >= targetTime)
        {
            int2 playerPosition = playerEntityQuery.GetSingleton<PlayerComponent>().position;
            GridComponent grid = gridEntityQuery.GetSingleton<GridComponent>();

            PathfindingJob job = new PathfindingJob
            {
                playerPosition = playerPosition,
                grid = grid,
            };

            JobHandle handle = job.ScheduleParallel(state.Dependency);
            state.Dependency = handle;

            timer = 0;
        }
    }
}