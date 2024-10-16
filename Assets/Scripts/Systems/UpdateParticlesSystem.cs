using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using SphereCollider = Unity.Physics.SphereCollider;

[BurstCompile]
[UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(GridEnemyPositionUpdateSystem))]
public partial struct UpdateParticlesSystem : ISystem
{
    private EntityQuery gridEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<GridComponent>()
            .Build(ref state);

        state.RequireForUpdate<ParticleReference>();
        state.RequireForUpdate<PlayerAliveComponent>();
        state.RequireForUpdate(gridEntityQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        RefRW<GridComponent> gridComponent = gridEntityQuery.GetSingletonRW<GridComponent>();

        foreach (var (transform, particleReference) in SystemAPI.Query<RefRO<LocalTransform>, ParticleReference>())
        {
            ParticleSystem particleSystem = particleReference.value.GetComponent<ParticleSystem>();
            Particle[] particles = new Particle[particleSystem.main.maxParticles];

            int numberOfActiveParticles = particleSystem.GetParticles(particles);

            for (int i = 0; i < numberOfActiveParticles; i++)
            {
                Particle particle = particles[i];
                float particleSize = particle.GetCurrentSize(particleSystem);

                CalculateParticleCollisionWithEnemy(ref state, transform, particle, gridComponent, particleSize);
            }

            float3 newParticlePosition = new float3(transform.ValueRO.Position.x, 0, transform.ValueRO.Position.z);

            if (particleReference.updateTransform)
            {
                UpdateTransform(particleReference, newParticlePosition);
            }
            else if (particleSystem.time >= (particleSystem.main.duration - 0.05f))
            {
                particleSystem.Clear();
                UpdateTransform(particleReference, newParticlePosition);
            }
        }
    }

    [BurstCompile]
    private void CalculateParticleCollisionWithEnemy(ref SystemState state, RefRO<LocalTransform> transform, Particle particle, RefRW<GridComponent> gridComponent, float particleSize)
    {
        float3 particleWorldPosition = transform.ValueRO.TransformPoint(new float3(particle.position.x, 0, -particle.position.y));
        int2 particleGridPosition = new int2((int)math.round(particleWorldPosition.x), (int)math.round(particleWorldPosition.z));

        if (!gridComponent.ValueRO.enemyPositions.ContainsKey(particleGridPosition)) return;

        foreach (Entity enemy in gridComponent.ValueRO.enemyPositions[particleGridPosition])
        {
            // TODO: Make it so this isn't needed later
            if (!SystemAPI.Exists(enemy)) return;

            LocalTransform enemyTransform = SystemAPI.GetComponent<LocalTransform>(enemy);
            PhysicsCollider enemyCollider = SystemAPI.GetComponent<PhysicsCollider>(enemy);
            SphereCollider sphereCollider = enemyCollider.Value.As<SphereCollider>();

            float colliderRadius = sphereCollider.Radius;

            if (math.distance(particleWorldPosition, enemyTransform.Position) > ((particleSize / 2) + colliderRadius)) continue;

            Debug.Log("collision");
            // Collision happened!
            // TODO: Handle collision
        }
    }

    [BurstCompile]
    private void UpdateTransform(ParticleReference particleReference, float3 position)
    {
        particleReference.value.transform.position = position;
    }
}