using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemySpawnerSystem))]
public partial struct InstantiateOrPoolHealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HealthComponent>();
        state.RequireForUpdate<UIPrefabs>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (health, transform, healthBarOffset, entity) in SystemAPI.Query<RefRO<HealthComponent>, RefRO<LocalTransform>, RefRO<HealthBarOffset>>()
            .WithNone<HealthBarUIReference, PlayerComponent>()
            .WithAll<MaterialChangedComponent>()
            .WithEntityAccess())
        {
            float3 spawnPosition = transform.ValueRO.Position + healthBarOffset.ValueRO.value;

            GameObject newHealthBar = HealthBarPoolManager.Instance.GetHealthBar(spawnPosition);

            ecb.AddComponent(entity, new HealthBarUIReference
            {
                value = newHealthBar
            });

            SetHealthBar(newHealthBar, health.ValueRO);
        }
    }

    [BurstCompile]
    public void SetHealthBar(GameObject healthBarObject, HealthComponent health)
    {
        Slider hpBarSlider = healthBarObject.GetComponent<Slider>();
        hpBarSlider.minValue = 0;
        hpBarSlider.maxValue = health.maxHitPoints;
        hpBarSlider.value = health.HitPoints;
    }
}