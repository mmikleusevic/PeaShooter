using Components;
using Helpers;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct UpdateUIBarValueSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UIBarUIReference>();
            state.RequireForUpdate<PlayerAliveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach ((RefRO<HealthComponent> healthRO, UIBarUIReference uiBarReference) in SystemAPI
                         .Query<RefRO<HealthComponent>, UIBarUIReference>()
                         .WithChangeFilter<HealthComponent>())
            {
                UIBarUtility.SetSliderValues(uiBarReference.hpSlider, 0, healthRO.ValueRO.maxHitPoints,
                    healthRO.ValueRO.HitPoints);
            }

            foreach ((RefRO<BarrierComponent> barrierRO, UIBarUIReference uiBarReference) in SystemAPI
                         .Query<RefRO<BarrierComponent>, UIBarUIReference>()
                         .WithChangeFilter<BarrierComponent>())
            {
                UIBarUtility.SetSliderValues(uiBarReference.barrierSlider, 0, barrierRO.ValueRO.maxBarrierValue,
                    barrierRO.ValueRO.BarrierValue);
            }

            foreach ((RefRO<LocalTransform> localTransformRO, RefRO<UIBarOffset> uiBarOffsetRO,
                         UIBarUIReference uiBarReference) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<UIBarOffset>, UIBarUIReference>()
                         .WithChangeFilter<LocalTransform>())
            {
                UIBarUtility.UpdateTransform(uiBarReference.gameObject, localTransformRO.ValueRO.Position,
                    uiBarOffsetRO.ValueRO.offset);
            }
        }
    }
}