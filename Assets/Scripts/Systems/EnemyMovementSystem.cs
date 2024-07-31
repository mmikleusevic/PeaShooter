using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct EnemyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        EnemyMovementJob job = new EnemyMovementJob
        {
            deltaTime = deltaTime,
        };

        JobHandle handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}