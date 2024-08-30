using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial struct TargetingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        TargetingSystemJob job = new TargetingSystemJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;
    }
}
