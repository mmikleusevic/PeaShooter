using Components;
using Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct TriggerPickUpSystem : ISystem
    {
        private ComponentLookup<HealthComponent> healthComponentLookup;
        private ComponentLookup<BarrierComponent> barrierComponentLookup;
        private ComponentLookup<HealthPickUpComponent> healthPickUpComponentLookup;
        private ComponentLookup<BarrierPickUpComponent> barrierPickUpComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            healthComponentLookup = state.GetComponentLookup<HealthComponent>();
            barrierComponentLookup = state.GetComponentLookup<BarrierComponent>();
            healthPickUpComponentLookup = state.GetComponentLookup<HealthPickUpComponent>();
            barrierPickUpComponentLookup = state.GetComponentLookup<BarrierPickUpComponent>();

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            healthComponentLookup.Update(ref state);
            barrierComponentLookup.Update(ref state);
            healthPickUpComponentLookup.Update(ref state);
            barrierPickUpComponentLookup.Update(ref state);

            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            TriggerPickUpJob job = new TriggerPickUpJob
            {
                ecb = ecb,
                healthComponentLookup = healthComponentLookup,
                barrierComponentLookup = barrierComponentLookup,
                healthPickUpComponentLookup = healthPickUpComponentLookup,
                barrierPickUpComponentLookup = barrierPickUpComponentLookup
            };

            SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

            JobHandle handle = job.Schedule(simulationSingleton, state.Dependency);
            state.Dependency = handle;
        }
    }
}