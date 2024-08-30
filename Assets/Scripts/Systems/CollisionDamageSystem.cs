using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial class CollisionDamageSystem : SystemBase
{
    public event Action OnPlayerDied;

    protected override void OnCreate()
    {
        RequireForUpdate<SimulationSingleton>();
        RequireForUpdate<HealthComponent>();
        RequireForUpdate<EnemyDamageComponent>();
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.TryGetSingleton(out PlayerDeadComponent playerDead))
        {
            Enabled = false;
            OnPlayerDied?.Invoke();
            return;
        }

        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        CollisionDamageJob job = new CollisionDamageJob
        {
            projectileLookup = SystemAPI.GetComponentLookup<ProjectileComponent>(),
            healthLookup = SystemAPI.GetComponentLookup<HealthComponent>(),
            ecb = ecb,
            enemyDamageLookup = SystemAPI.GetComponentLookup<EnemyDamageComponent>(true),
            activeForCollisionLookup = SystemAPI.GetComponentLookup<ActiveForCollisionComponent>(true),
            deltaTime = SystemAPI.Time.DeltaTime
        };

        SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, Dependency);
        Dependency = handle;
    }
}