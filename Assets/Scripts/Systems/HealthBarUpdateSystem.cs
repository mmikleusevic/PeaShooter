using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct UpdateHealthBarValueSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
            state.RequireForUpdate<HealthBarUIReference>();
            state.RequireForUpdate<PlayerAliveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach ((RefRO<HealthComponent> healthRO, HealthBarUIReference healthBarUI) in SystemAPI
                         .Query<RefRO<HealthComponent>, HealthBarUIReference>()
                         .WithChangeFilter<HealthComponent>())
                SetHealthBar(healthBarUI.gameObject, healthRO.ValueRO);

            foreach ((RefRO<LocalTransform> localTransformRO, RefRO<HealthBarOffset> healthBarOffsetRO,
                         HealthBarUIReference healthBarUI) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<HealthBarOffset>, HealthBarUIReference>()
                         .WithChangeFilter<LocalTransform>())
            {
                SetTransform(healthBarUI.gameObject, localTransformRO.ValueRO, healthBarOffsetRO.ValueRO);
            }
        }

        [BurstCompile]
        private void SetHealthBar(GameObject healthBarUI, HealthComponent health)
        {
            Slider hpBarSlider = healthBarUI.GetComponentInChildren<Slider>();
            hpBarSlider.minValue = 0;
            hpBarSlider.maxValue = health.maxHitPoints;
            hpBarSlider.value = health.HitPoints;
        }

        [BurstCompile]
        private void SetTransform(GameObject healthBarUI, LocalTransform localTransform,
            HealthBarOffset healthBarOffset)
        {
            float3 healthBarPosition = localTransform.Position + healthBarOffset.offset;
            healthBarUI.gameObject.transform.position = healthBarPosition;
        }
    }
}