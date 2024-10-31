using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

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
        foreach ((RefRO<HealthComponent> health, HealthBarUIReference healthBarUI) in SystemAPI
                     .Query<RefRO<HealthComponent>, HealthBarUIReference>()
                     .WithChangeFilter<HealthComponent>())
            SetHealthBar(healthBarUI.value, health.ValueRO);

        foreach ((RefRO<LocalTransform> transform, RefRO<HealthBarOffset> healthBarOffset,
                     HealthBarUIReference healthBarUI) in SystemAPI
                     .Query<RefRO<LocalTransform>, RefRO<HealthBarOffset>, HealthBarUIReference>()
                     .WithChangeFilter<LocalTransform>())
        {
            float3 healthBarPosition = transform.ValueRO.Position + healthBarOffset.ValueRO.value;
            healthBarUI.value.transform.position = healthBarPosition;
        }
    }

    [BurstCompile]
    public void SetHealthBar(GameObject healthBarCanvasObject, HealthComponent health)
    {
        Slider hpBarSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
        hpBarSlider.minValue = 0;
        hpBarSlider.maxValue = health.maxHitPoints;
        hpBarSlider.value = health.HitPoints;
    }
}