using Managers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial struct HealthBarInstantiateOrPoolSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerAliveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((RefRO<HealthComponent> health, RefRO<LocalTransform> transform,
                         RefRO<HealthBarOffset> healthBarOffset, Entity entity) in SystemAPI
                         .Query<RefRO<HealthComponent>, RefRO<LocalTransform>, RefRO<HealthBarOffset>>()
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
}