using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.ParticleSystem;


namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(DestroySystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct UpdateParticlesSystem : ISystem
    {
        private EntityQuery gridEntityQuery;
        private EntityQuery playerEntityQuery;
        private ComponentLookup<HealthComponent> healthComponentLookup;
        private ComponentLookup<BarrierComponent> barrierComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GridComponent>()
                .Build(ref state);

            playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerAliveComponent, LocalTransform>()
                .Build(ref state);

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ParticleObjectReferenceComponent>();
            state.RequireForUpdate(playerEntityQuery);
            state.RequireForUpdate(gridEntityQuery);

            healthComponentLookup = state.GetComponentLookup<HealthComponent>();
            barrierComponentLookup = state.GetComponentLookup<BarrierComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();
            healthComponentLookup.Update(ref state);
            barrierComponentLookup.Update(ref state);

            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (particleReference, localTransformRW, abilityComponent) in SystemAPI
                         .Query<ParticleObjectReferenceComponent, RefRW<LocalTransform>, RefRO<AbilityComponent>>())
            {
                ParticleSystem particleSystem = particleReference.gameObject.GetComponent<ParticleSystem>();

                if (particleSystem.particleCount > 0)
                {
                    NativeArray<Particle> particles =
                        new NativeArray<Particle>(particleSystem.particleCount, Allocator.TempJob);
                    particleSystem.GetParticles(particles);

                    ParticlesCollisionJob job = new ParticlesCollisionJob
                    {
                        ecb = ecb,
                        enemyPositions = gridComponent.enemyPositions.AsReadOnly(),
                        healthComponentLookup = healthComponentLookup,
                        barrierComponentLookup = barrierComponentLookup,
                        particles = particles,
                        abilityComponent = abilityComponent.ValueRO,
                        localTransform = localTransformRW.ValueRO,
                        deltaTime = SystemAPI.Time.DeltaTime
                    };

                    JobHandle jobHandle = job.Schedule(state.Dependency);
                    JobHandle combinedDependencies =
                        JobHandle.CombineDependencies(jobHandle, particles.Dispose(jobHandle));
                    state.Dependency = combinedDependencies;
                }

                LocalTransform playerTransform = playerEntityQuery.GetSingleton<LocalTransform>();

                if (particleReference.updateTransform == 1)
                {
                    UpdateTransform(particleReference, playerTransform.Position, ref localTransformRW.ValueRW);
                }
                else if (particleSystem.time >= particleSystem.main.duration - 0.05f)
                {
                    particleSystem.Clear();
                    UpdateTransform(particleReference, playerTransform.Position, ref localTransformRW.ValueRW);
                }
            }
        }

        [BurstCompile]
        private void UpdateTransform(ParticleObjectReferenceComponent particleReference, float3 position,
            ref LocalTransform localTransform)
        {
            particleReference.gameObject.transform.position = position;
            localTransform.Position = position;
        }
    }
}