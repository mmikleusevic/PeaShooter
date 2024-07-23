using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial struct ObstacleUpdateSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        JobHandle job = new ObstacleUpdateJob().ScheduleParallel(state.Dependency);

        state.Dependency = job;
    }
}