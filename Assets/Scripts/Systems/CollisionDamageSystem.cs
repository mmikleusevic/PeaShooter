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

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad += OnGameLoad;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad -= OnGameLoad;
        }
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.HasSingleton<PlayerDeadComponent>())
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
            targetLookup = SystemAPI.GetComponentLookup<TargetComponent>(true),
            obstacleLookup = SystemAPI.GetComponentLookup<ObstacleComponent>(true),
            enemyDamageLookup = SystemAPI.GetComponentLookup<EnemyDamageComponent>(true),
            activeForCollisionLookup = SystemAPI.GetComponentLookup<ActiveForCollisionComponent>(true),
            deltaTime = SystemAPI.Time.DeltaTime
        };

        SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, Dependency);
        Dependency = handle;
    }

    private void OnGameLoad()
    {
        Enabled = true;
    }
}