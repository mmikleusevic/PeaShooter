using System;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial class PlayerHealthSystem : SystemBase
{
    public event Action OnPlayerDied;
    public event Action<float> OnHealthChanged;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireForUpdate<PlayerAliveComponent>();
        RequireForUpdate<HealthComponent>();
    }

    protected override void OnUpdate()
    {
        foreach (RefRO<HealthComponent> playerHealth in SystemAPI.Query<RefRO<HealthComponent>>()
                     .WithChangeFilter<HealthComponent>()
                     .WithAll<PlayerAliveComponent>())
        {
            OnHealthChanged?.Invoke(playerHealth.ValueRO.HitPoints);

            if (playerHealth.ValueRO.IsDead) OnPlayerDied?.Invoke();
        }
    }
}