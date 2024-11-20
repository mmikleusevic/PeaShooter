using Managers;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(UpdateHealthBarValueSystem))]
    public partial struct HealthBarDisableAndPoolSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HealthBarUIReference>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((HealthBarUIReference healthBarUI, Entity enemyEntity) in SystemAPI.Query<HealthBarUIReference>()
                         .WithNone<LocalTransform>()
                         .WithEntityAccess())
            {
                HealthBarPoolManager.Instance.ReturnHealthBar(healthBarUI.gameObject);
                ecb.RemoveComponent<HealthBarUIReference>(enemyEntity);
            }
        }
    }
}