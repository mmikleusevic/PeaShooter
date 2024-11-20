using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct CollisionDamageSystem : ISystem
    {
        private ComponentLookup<ProjectileComponent> projectileComponentLookup;
        private ComponentLookup<HealthComponent> healthComponentLookup;
        private ComponentLookup<TargetComponent> targetComponentLookup;
        private ComponentLookup<AbilityComponent> abilityComponentLookup;
        private ComponentLookup<ProjectileAbilityComponent> projectileAbilityComponentLookup;
        private ComponentLookup<ObstacleComponent> obstacleComponentLookup;
        private ComponentLookup<EnemyDamageComponent> enemyDamageComponentLookup;
        private ComponentLookup<CollisionActiveComponent> activeForCollisionComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PlayerAliveComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            projectileComponentLookup = state.GetComponentLookup<ProjectileComponent>();
            healthComponentLookup = state.GetComponentLookup<HealthComponent>();
            targetComponentLookup = state.GetComponentLookup<TargetComponent>(true);
            abilityComponentLookup = state.GetComponentLookup<AbilityComponent>(true);
            projectileAbilityComponentLookup = state.GetComponentLookup<ProjectileAbilityComponent>(true);
            obstacleComponentLookup = state.GetComponentLookup<ObstacleComponent>(true);
            enemyDamageComponentLookup = state.GetComponentLookup<EnemyDamageComponent>(true);
            activeForCollisionComponentLookup = state.GetComponentLookup<CollisionActiveComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            projectileComponentLookup.Update(ref state);
            healthComponentLookup.Update(ref state);
            targetComponentLookup.Update(ref state);
            abilityComponentLookup.Update(ref state);
            projectileAbilityComponentLookup.Update(ref state);
            obstacleComponentLookup.Update(ref state);
            enemyDamageComponentLookup.Update(ref state);
            activeForCollisionComponentLookup.Update(ref state);

            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            CollisionDamageJob job = new CollisionDamageJob
            {
                ecb = ecb,
                projectileComponentLookup = projectileComponentLookup,
                healthComponentLookup = healthComponentLookup,
                targetComponentLookup = targetComponentLookup,
                abilityComponentLookup = abilityComponentLookup,
                projectileAbilityComponentLookup = projectileAbilityComponentLookup,
                obstacleComponentLookup = obstacleComponentLookup,
                enemyDamageComponentLookup = enemyDamageComponentLookup,
                activeForCollisionComponentLookup = activeForCollisionComponentLookup,
                deltaTime = SystemAPI.Time.fixedDeltaTime
            };

            SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

            JobHandle handle = job.Schedule(simulationSingleton, state.Dependency);
            state.Dependency = handle;
        }
    }
}