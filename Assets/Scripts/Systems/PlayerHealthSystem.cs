using System;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial class PlayerHealthSystem : SystemBase
{
    public event Action<float> OnHealthChanged;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireForUpdate<PlayerComponent>();
        RequireForUpdate<HealthComponent>();
    }

    protected override void OnUpdate()
    {
        foreach (var playerHealth in SystemAPI.Query<RefRO<HealthComponent>>()
            .WithChangeFilter<HealthComponent>()
            .WithNone<PlayerDeadComponent>()
            .WithAll<PlayerComponent>())
        {
            OnHealthChanged?.Invoke(playerHealth.ValueRO.HitPoints);
        }
    }
}