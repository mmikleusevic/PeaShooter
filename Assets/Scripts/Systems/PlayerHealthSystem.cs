using System;
using Components;
using Unity.Entities;

namespace Systems
{
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
            foreach (RefRO<HealthComponent> playerHealthRO in SystemAPI.Query<RefRO<HealthComponent>>()
                         .WithChangeFilter<HealthComponent>()
                         .WithAll<PlayerAliveComponent>())
            {
                OnHealthChanged?.Invoke(playerHealthRO.ValueRO.HitPoints);

                if (playerHealthRO.ValueRO.IsDead) OnPlayerDied?.Invoke();
            }
        }
    }
}