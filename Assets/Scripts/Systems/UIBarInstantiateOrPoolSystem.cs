using Components;
using Helpers;
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
    public partial struct UIBarInstantiateOrPoolSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
            state.RequireForUpdate<BarrierComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerAliveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((RefRO<HealthComponent> healthComponentRO, RefRO<BarrierComponent> barrierComponentRO,
                         RefRO<LocalTransform> localTransformRO,
                         RefRO<UIBarOffsetComponent> uiBarOffsetComponentRO, Entity enemyEntity) in SystemAPI
                         .Query<RefRO<HealthComponent>, RefRO<BarrierComponent>, RefRO<LocalTransform>,
                             RefRO<UIBarOffsetComponent>>()
                         .WithNone<UIBarUIReferenceComponent, PlayerComponent>()
                         .WithAll<MaterialChangedComponent>()
                         .WithEntityAccess())
            {
                float3 spawnPosition = localTransformRO.ValueRO.Position + uiBarOffsetComponentRO.ValueRO.offset;

                GameObject uiBarGameObject = UIBarPoolManager.Instance.GetUIBar(spawnPosition);

                Slider[] sliders = uiBarGameObject.gameObject.GetComponentsInChildren<Slider>();

                Slider healthSlider = sliders[0];
                Slider barrierSlider = sliders[1];

                ecb.AddComponent(enemyEntity, new UIBarUIReferenceComponent
                {
                    gameObject = uiBarGameObject,
                    hpSlider = healthSlider,
                    barrierSlider = barrierSlider
                });

                UIBarUtility.SetSliderValues(healthSlider, 0, healthComponentRO.ValueRO.maxHitPoints,
                    healthComponentRO.ValueRO.HitPoints);
                UIBarUtility.SetSliderValues(barrierSlider, 0, barrierComponentRO.ValueRO.maxBarrierValue,
                    barrierComponentRO.ValueRO.BarrierValue);
            }
        }
    }
}