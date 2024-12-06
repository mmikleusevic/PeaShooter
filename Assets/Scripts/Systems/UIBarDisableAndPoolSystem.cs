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
            state.RequireForUpdate<UIBarUIReference>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((UIBarUIReference uiBarUI, Entity enemyEntity) in SystemAPI.Query<UIBarUIReference>()
                         .WithNone<LocalTransform>()
                         .WithEntityAccess())
            {
                UIBarPoolManager.Instance.ReturnUIBar(uiBarUI.gameObject);
                ecb.RemoveComponent<UIBarUIReference>(enemyEntity);
            }
        }
    }
}