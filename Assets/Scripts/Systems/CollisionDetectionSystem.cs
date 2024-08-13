using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct CollisionDetectionSystem : ISystem
{
    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        CollisionEventJob job = new CollisionEventJob
        {
            PlayerLookup = SystemAPI.GetComponentLookup<PlayerComponent>(true),
            EnemyLookup = SystemAPI.GetComponentLookup<EnemyComponent>(true)
        };

        SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, state.Dependency);
        state.Dependency = handle;
    }
}
