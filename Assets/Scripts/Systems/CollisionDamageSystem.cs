using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct CollisionDamageSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<PlayerAliveComponent>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        CollisionDamageJob job = new CollisionDamageJob
        {
            ecb = ecb,
            projectileLookup = SystemAPI.GetComponentLookup<ProjectileComponent>(),
            healthLookup = SystemAPI.GetComponentLookup<HealthComponent>(),
            targetLookup = SystemAPI.GetComponentLookup<TargetComponent>(true),
            abilityLookup = SystemAPI.GetComponentLookup<AbilityComponent>(true),
            projectileAbilityLookup = SystemAPI.GetComponentLookup<ProjectileAbilityComponent>(true),
            obstacleLookup = SystemAPI.GetComponentLookup<ObstacleComponent>(true),
            enemyDamageLookup = SystemAPI.GetComponentLookup<EnemyDamageComponent>(true),
            activeForCollisionLookup = SystemAPI.GetComponentLookup<ActiveForCollisionComponent>(true),
            deltaTime = SystemAPI.Time.DeltaTime
        };

        SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, state.Dependency);
        state.Dependency = handle;
    }
}