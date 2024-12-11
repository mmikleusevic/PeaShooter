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
            state.RequireForUpdate<UIBarUIReferenceComponent>();
            state.RequireForUpdate<PlayerAliveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach ((RefRO<HealthComponent> healthRO, UIBarUIReferenceComponent uiBarReferenceComponent) in SystemAPI
                         .Query<RefRO<HealthComponent>, UIBarUIReferenceComponent>()
                         .WithChangeFilter<HealthComponent>())
            {
                UIBarUtility.SetSliderValues(uiBarReferenceComponent.hpSlider, 0, healthRO.ValueRO.maxHitPoints,
                    healthRO.ValueRO.HitPoints);
            }

            foreach ((RefRO<BarrierComponent> barrierRO, UIBarUIReferenceComponent uiBarReferenceComponent) in SystemAPI
                         .Query<RefRO<BarrierComponent>, UIBarUIReferenceComponent>()
                         .WithChangeFilter<BarrierComponent>())
            {
                UIBarUtility.SetSliderValues(uiBarReferenceComponent.barrierSlider, 0,
                    barrierRO.ValueRO.maxBarrierValue,
                    barrierRO.ValueRO.BarrierValue);
            }

            foreach ((RefRO<LocalTransform> localTransformRO, RefRO<UIBarOffsetComponent> uiBarOffsetComponentRO,
                         UIBarUIReferenceComponent uiBarReferenceComponent) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<UIBarOffsetComponent>, UIBarUIReferenceComponent>()
                         .WithChangeFilter<LocalTransform>())
            {
                UIBarUtility.UpdateTransform(uiBarReferenceComponent.gameObject, localTransformRO.ValueRO.Position,
                    uiBarOffsetComponentRO.ValueRO.offset);
            }
        }
    }
}