using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct ObstacleUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        ObstacleUpdateJob job = new ObstacleUpdateJob();

        job.ScheduleParallel();

        state.World.CreateSystem<PathfindingSystem>();
    }
}