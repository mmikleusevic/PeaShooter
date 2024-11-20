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

            foreach ((RefRO<HealthComponent> healthRO, RefRO<LocalTransform> localTransformRO,
                         RefRO<HealthBarOffset> healthBarOffsetRO, Entity enemyEntity) in SystemAPI
                         .Query<RefRO<HealthComponent>, RefRO<LocalTransform>, RefRO<HealthBarOffset>>()
                         .WithNone<HealthBarUIReference, PlayerComponent>()
                         .WithAll<MaterialChangedComponent>()
                         .WithEntityAccess())
            {
                float3 spawnPosition = localTransformRO.ValueRO.Position + healthBarOffsetRO.ValueRO.offset;

                GameObject healthBarGameObject = HealthBarPoolManager.Instance.GetHealthBar(spawnPosition);

                ecb.AddComponent(enemyEntity, new HealthBarUIReference
                {
                    gameObject = healthBarGameObject
                });

                SetHealthBar(healthBarGameObject, healthRO.ValueRO);
            }
        }

        [BurstCompile]
        private void SetHealthBar(GameObject healthBarGameObject, HealthComponent health)
        {
            Slider hpBarSlider = healthBarGameObject.GetComponent<Slider>();
            hpBarSlider.minValue = 0;
            hpBarSlider.maxValue = health.maxHitPoints;
            hpBarSlider.value = health.HitPoints;
        }
    }
}