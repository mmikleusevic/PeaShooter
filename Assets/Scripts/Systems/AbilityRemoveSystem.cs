using Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial struct AbilityRemoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AbilityRemoveComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (abilityComponent, abilityEntity) in SystemAPI.Query<RefRW<AbilityComponent>>()
                         .WithNone<ParticleObjectReferenceComponent>()
                         .WithAll<AbilityRemoveComponent>()
                         .WithEntityAccess())
            {
                abilityComponent.ValueRW.Dispose();

                ecb.AddComponent<DestroyComponent>(abilityEntity);
            }

            foreach (var (abilityComponent, particleComponent, abilityEntity) in SystemAPI
                         .Query<RefRW<AbilityComponent>, ParticleObjectReferenceComponent>()
                         .WithAll<AbilityRemoveComponent>()
                         .WithEntityAccess())
            {
                abilityComponent.ValueRW.Dispose();

                ecb.AddComponent<DestroyComponent>(abilityEntity);
                ecb.RemoveComponent<ParticleObjectReferenceComponent>(abilityEntity);
                Object.Destroy(particleComponent.gameObject);
            }
        }
    }
}