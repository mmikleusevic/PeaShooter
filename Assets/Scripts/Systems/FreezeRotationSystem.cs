using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct FreezeRotationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        FreezeRotationJob job = new FreezeRotationJob();
        job.ScheduleParallel();
    }
}
