using Components;
using Managers;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(UpdateUIBarValueSystem))]
    public partial struct UIBarDisableAndPoolSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UIBarUIReferenceComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((UIBarUIReferenceComponent uiBarUI, Entity enemyEntity) in SystemAPI
                         .Query<UIBarUIReferenceComponent>()
                         .WithNone<LocalTransform>()
                         .WithEntityAccess())
            {
                UIBarPoolManager.Instance.ReturnUIBar(uiBarUI.gameObject);
                ecb.RemoveComponent<UIBarUIReferenceComponent>(enemyEntity);
            }
        }
    }
}