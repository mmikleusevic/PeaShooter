using System;
using Components;
using Unity.Entities;

namespace Systems
{
    public partial class PlayerBarrierSystem : SystemBase
    {
        public event Action<float> OnBarrierChanged;

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<PlayerAliveComponent>();
            RequireForUpdate<BarrierComponent>();
        }

        protected override void OnUpdate()
        {
            foreach (RefRO<BarrierComponent> playerBarrierComponentRO in SystemAPI.Query<RefRO<BarrierComponent>>()
                         .WithChangeFilter<BarrierComponent>()
                         .WithAll<PlayerAliveComponent>())
            {
                OnBarrierChanged?.Invoke(playerBarrierComponentRO.ValueRO.BarrierValue);
            }
        }
    }
}