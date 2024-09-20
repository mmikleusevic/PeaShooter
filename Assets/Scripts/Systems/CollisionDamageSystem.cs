using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[UpdateBefore(typeof(ProjectileDisablingSystem))]
public partial struct CollisionDamageSystem : ISystem
{
    private EntityQuery simulationEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        simulationEntityQuery = new EntityQueryBuilder(Allocator.Temp)
           .WithAll<SimulationSingleton>()
           .Build(state.EntityManager);

        state.RequireForUpdate(simulationEntityQuery);
        state.RequireForUpdate<HealthComponent>();
        state.RequireForUpdate<EnemyDamageComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.HasSingleton<PlayerDeadComponent>()) return;

        EndFixedStepSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        CollisionDamageJob job = new CollisionDamageJob
        {
            ecb = ecb,
            projectileLookup = SystemAPI.GetComponentLookup<ProjectileComponent>(),
            healthLookup = SystemAPI.GetComponentLookup<HealthComponent>(),
            targetLookup = SystemAPI.GetComponentLookup<TargetComponent>(true),
            obstacleLookup = SystemAPI.GetComponentLookup<ObstacleComponent>(true),
            enemyDamageLookup = SystemAPI.GetComponentLookup<EnemyDamageComponent>(true),
            activeForCollisionLookup = SystemAPI.GetComponentLookup<ActiveForCollisionComponent>(true),
            deltaTime = SystemAPI.Time.DeltaTime
        };

        SimulationSingleton simulationSingleton = simulationEntityQuery.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, state.Dependency);
        state.Dependency = handle;
    }
}