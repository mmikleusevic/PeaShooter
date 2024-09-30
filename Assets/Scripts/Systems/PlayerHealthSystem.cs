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
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        foreach (var playerHealth in SystemAPI.Query<RefRO<HealthComponent>>()
            .WithChangeFilter<HealthComponent>()
            .WithAll<PlayerComponent>())
        {
            OnHealthChanged?.Invoke(playerHealth.ValueRO.HitPoints);
        }
    }
}