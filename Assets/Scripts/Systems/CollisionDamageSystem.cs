using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct CollisionDamageSystem : ISystem
{
    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<PlayerHealthComponent>();
        state.RequireForUpdate<EnemyDamageComponent>();
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.GetSingleton<PlayerHealthComponent>().isDead == true)
        {
            state.Enabled = false;
        }

        CollisionDamageJob job = new CollisionDamageJob
        {
            playerHealthLookup = SystemAPI.GetComponentLookup<PlayerHealthComponent>(),
            enemyDamageLookup = SystemAPI.GetComponentLookup<EnemyDamageComponent>(true)
        };

        SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, state.Dependency);
        state.Dependency = handle;
    }
}