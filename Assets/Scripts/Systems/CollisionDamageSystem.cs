using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PlayerMovementSystem))]
public partial class CollisionDamageSystem : SystemBase
{
    public event Action OnPlayerDied;
    private EntityQuery playerEntityQuery;

    protected override void OnCreate()
    {
        playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
           .WithAll<SimulationSingleton>()
           .Build(EntityManager);

        RequireForUpdate(playerEntityQuery);
        RequireForUpdate<HealthComponent>();
        RequireForUpdate<EnemyDamageComponent>();
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.HasSingleton<PlayerDeadComponent>())
        {
            OnPlayerDied?.Invoke();
            return;
        }

        BeginFixedStepSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

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

        SimulationSingleton simulationSingleton = playerEntityQuery.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, Dependency);
        Dependency = handle;
    }
}