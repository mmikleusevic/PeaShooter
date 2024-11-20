using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Object = UnityEngine.Object;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(UpdateHealthBarValueSystem))]
    public partial struct ParticleDisableSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ParticleObjectReferenceComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((ParticleObjectReferenceComponent particleComponent, Entity particleEntity) in SystemAPI
                         .Query<ParticleObjectReferenceComponent>()
                         .WithNone<LocalTransform>()
                         .WithEntityAccess())
            {
                Object.Destroy(particleComponent.gameObject);
                ecb.RemoveComponent<ParticleObjectReferenceComponent>(particleEntity);
            }
        }
    }
}