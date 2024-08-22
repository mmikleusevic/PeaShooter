using System;
using Unity.Burst;
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
        RequireForUpdate<PlayerHealthComponent>();
        RequireForUpdate<EnemyDamageComponent>();
    }

    protected override void OnUpdate()
    {
        CollisionDamageJob job = new CollisionDamageJob
        {
            enemyDamageLookup = SystemAPI.GetComponentLookup<EnemyDamageComponent>(true),
            deltaTime = SystemAPI.Time.DeltaTime,
            playerHealthLookup = SystemAPI.GetComponentLookup<PlayerHealthComponent>()
        };

        SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

        JobHandle handle = job.Schedule(simulationSingleton, Dependency);
        Dependency = handle;
        Dependency.Complete();

        if (SystemAPI.GetSingleton<PlayerHealthComponent>().IsDead == true)
        {
            Enabled = false;
            OnPlayerDied?.Invoke();
            return;
        }
    }
}