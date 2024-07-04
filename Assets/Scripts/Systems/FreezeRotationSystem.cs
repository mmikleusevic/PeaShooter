using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct FreezeRotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        FreezeRotationJob job = new FreezeRotationJob();
        JobHandle handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}
