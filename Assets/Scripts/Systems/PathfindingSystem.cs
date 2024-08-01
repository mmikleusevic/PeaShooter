using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct PathfindingSystem : ISystem
{
    private float timer;
    private float targetTime;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();
        state.RequireForUpdate<GridComponent>();

        timer = 0.5f;
        targetTime = timer;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        timer += SystemAPI.Time.DeltaTime;

        if (timer >= targetTime)
        {
            int2 playerPosition = SystemAPI.GetSingleton<PlayerComponent>().position;
            GridComponent grid = SystemAPI.GetSingleton<GridComponent>();

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