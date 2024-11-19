using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Particle = UnityEngine.ParticleSystem.Particle;

namespace Jobs
{
    [BurstCompile]
    public partial struct ParticlesUpdateJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        public ComponentLookup<HealthComponent> healthLookup;

        [ReadOnly] public NativeParallelMultiHashMap<int2, Entity>.ReadOnly enemyPositions;
        [ReadOnly] public NativeArray<Particle> particles;

        public void Execute(in AbilityComponent abilityComponent, in LocalTransform transform)
        {
            foreach (Particle particle in particles)
            {
                float3 localParticlePosition =
                    new float3(particle.position.x, particle.position.z, -particle.position.y);
                float3 particleWorldPosition = localParticlePosition + transform.Position;
                int2 particleGridPosition = new int2((int)math.round(particleWorldPosition.x),
                    (int)math.round(particleWorldPosition.z));

                if (particleWorldPosition.Equals(default) ||
                    !enemyPositions.ContainsKey(particleGridPosition)) continue;

                if (enemyPositions.TryGetFirstValue(particleGridPosition, out Entity enemy, out var iterator))
                {
                    do
                    {
                        if (healthLookup.HasComponent(enemy))
                        {
                            RefRW<HealthComponent> enemyHealth = healthLookup.GetRefRW(enemy);
                            enemyHealth.ValueRW.HitPoints -= abilityComponent.damage;

                            if (enemyHealth.ValueRO.HitPoints <= 0)
                            {
                                ecb.AddComponent(enemy, new EnemyDeadComponent());
                                ecb.SetComponent(enemy, new GridEnemyPositionUpdateComponent
                                {
                                    entity = enemy,
                                    oldPosition = particleGridPosition,
                                    status = UpdateStatus.Remove,
                                    position = particleGridPosition
                                });
                                ecb.AddComponent(enemy, new PositionChangedComponent());
                                ecb.AddComponent(enemy, new DestroyComponent());
                            }
                        }
                    } while (enemyPositions.TryGetNextValue(out enemy, ref iterator));
                }
            }
        }
    }
}