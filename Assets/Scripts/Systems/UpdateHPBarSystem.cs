using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup), OrderLast = true)]
public partial struct UpdateHPBarSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HealthComponent>();
        state.RequireForUpdate<UIPrefabs>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (health, transform, healthBarOffset, entity) in SystemAPI.Query<RefRO<HealthComponent>, RefRO<LocalTransform>, RefRO<HealthBarOffset>>()
            .WithNone<HealthBarUIReference, PlayerComponent>()
            .WithEntityAccess())
        {
            GameObject hpBarGO = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().hpBar;
            float3 spawnPosition = transform.ValueRO.Position + healthBarOffset.ValueRO.value;
            GameObject newHealthBar = Object.Instantiate(hpBarGO, spawnPosition, hpBarGO.transform.rotation);

            SetHealthBar(newHealthBar, health.ValueRO.HitPoints, health.ValueRO.maxHitPoints);
            ecb.AddComponent(entity, new HealthBarUIReference
            {
                value = newHealthBar
            });
        }

        foreach (var (transform, healthBarOffset, health, healthBarUI) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<HealthBarOffset>, RefRO<HealthComponent>, HealthBarUIReference>())
        {
            float3 healthBarPosition = transform.ValueRO.Position + healthBarOffset.ValueRO.value;
            healthBarUI.value.transform.position = healthBarPosition;
            SetHealthBar(healthBarUI.value, health.ValueRO.HitPoints, health.ValueRO.maxHitPoints);
        }

        foreach (var (healthBarUI, entity) in SystemAPI.Query<HealthBarUIReference>()
            .WithNone<LocalTransform>()
            .WithEntityAccess())
        {
            Object.Destroy(healthBarUI.value);
            ecb.RemoveComponent<HealthBarUIReference>(entity);
        }
    }

    private void SetHealthBar(GameObject healthBarCanvasObject, float currentHP, float maxHP)
    {
        var hpBarSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
        hpBarSlider.maxValue = 0;
        hpBarSlider.maxValue = maxHP;
        hpBarSlider.value = currentHP;
    }
}