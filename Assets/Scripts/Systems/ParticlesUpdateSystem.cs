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
        }

        public void OnUpdate(ref SystemState state)
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();
            healthComponentLookup.Update(ref state);

            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (particleReference, localTransformRW) in SystemAPI
                         .Query<ParticleObjectReferenceComponent, RefRW<LocalTransform>>())
            {
                ParticleSystem particleSystem = particleReference.gameObject.GetComponent<ParticleSystem>();

                if (particleSystem.particleCount > 0)
                {
                    NativeArray<Particle> particles =
                        new NativeArray<Particle>(particleSystem.particleCount, Allocator.TempJob);
                    particleSystem.GetParticles(particles);

                    ParticlesUpdateJob job = new ParticlesUpdateJob
                    {
                        ecb = ecb,
                        enemyPositions = gridComponent.enemyPositions.AsReadOnly(),
                        healthComponentLookup = healthComponentLookup,
                        particles = particles
                    };

                    JobHandle jobHandle = job.Schedule(state.Dependency);
                    state.Dependency = JobHandle.CombineDependencies(jobHandle, particles.Dispose(jobHandle));
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