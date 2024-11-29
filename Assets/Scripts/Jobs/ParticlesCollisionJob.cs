using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Particle = UnityEngine.ParticleSystem.Particle;

namespace Jobs
{
    [BurstCompile]
    [WithAll(typeof(ParticleObjectReferenceComponent))]
    public struct ParticlesCollisionJob : IJob
    {
        public EntityCommandBuffer ecb;
        public ComponentLookup<HealthComponent> healthComponentLookup;
        public ComponentLookup<BarrierComponent> barrierComponentLookup;

        [ReadOnly] public NativeParallelMultiHashMap<int2, Entity>.ReadOnly enemyPositions;
        [ReadOnly] public NativeArray<Particle> particles;
        [ReadOnly] public AbilityComponent abilityComponent;
        [ReadOnly] public LocalTransform localTransform;
        [ReadOnly] public float deltaTime;

        public void Execute()
        {
            foreach (Particle particle in particles)
            {
                float3 localParticlePosition =
                    new float3(particle.position.x, particle.position.z, -particle.position.y);
                float3 particleWorldPosition = localParticlePosition + localTransform.Position;
                int2 particleGridPosition = new int2((int)math.round(particleWorldPosition.x),
                    (int)math.round(particleWorldPosition.z));

                if (particleWorldPosition.Equals(default) ||
                    !enemyPositions.ContainsKey(particleGridPosition)) continue;

                if (enemyPositions.TryGetFirstValue(particleGridPosition, out Entity enemy, out var iterator))
                {
                    do
                    {
                        float damage = abilityComponent.damage * deltaTime;

                        if (barrierComponentLookup.HasComponent(enemy))
                        {
                            RefRW<BarrierComponent> barrierComponentRW = barrierComponentLookup.GetRefRW(enemy);
                            float damageToBarrier = math.min(barrierComponentRW.ValueRW.BarrierValue, damage);

                            barrierComponentRW.ValueRW.BarrierValue -= damageToBarrier;
                            damage -= damageToBarrier;
                        }

                        float damageToHealth = damage;

                        if (!healthComponentLookup.HasComponent(enemy)) continue;

                        RefRW<HealthComponent> enemyHealthComponent = healthComponentLookup.GetRefRW(enemy);
                        enemyHealthComponent.ValueRW.HitPoints -= damageToHealth;

                        if (enemyHealthComponent.ValueRO.IsDead) continue;

                        ecb.AddComponent<EnemyDeadComponent>(enemy);
                        ecb.SetComponent(enemy, new GridEnemyPositionUpdateComponent
                        {
                            enemyEntity = enemy,
                            oldPosition = particleGridPosition,
                            status = UpdateStatus.Remove,
                            position = particleGridPosition
                        });
                        ecb.AddComponent<PositionChangedComponent>(enemy);
                        ecb.AddComponent<DestroyComponent>(enemy);
                    } while (enemyPositions.TryGetNextValue(out enemy, ref iterator));
                }
            }
        }
    }
}