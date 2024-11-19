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
        private ComponentLookup<ProjectileComponent> projectileLookup;
        private ComponentLookup<HealthComponent> healthLookup;
        private ComponentLookup<TargetComponent> targetLookup;
        private ComponentLookup<AbilityComponent> abilityLookup;
        private ComponentLookup<ProjectileAbilityComponent> projectileAbilityLookup;
        private ComponentLookup<ObstacleComponent> obstacleLookup;
        private ComponentLookup<EnemyDamageComponent> enemyDamageLookup;
        private ComponentLookup<CollisionActiveComponent> activeForCollisionLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PlayerAliveComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            projectileLookup = state.GetComponentLookup<ProjectileComponent>();
            healthLookup = state.GetComponentLookup<HealthComponent>();
            targetLookup = state.GetComponentLookup<TargetComponent>(true);
            abilityLookup = state.GetComponentLookup<AbilityComponent>(true);
            projectileAbilityLookup = state.GetComponentLookup<ProjectileAbilityComponent>(true);
            obstacleLookup = state.GetComponentLookup<ObstacleComponent>(true);
            enemyDamageLookup = state.GetComponentLookup<EnemyDamageComponent>(true);
            activeForCollisionLookup = state.GetComponentLookup<CollisionActiveComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            projectileLookup.Update(ref state);
            healthLookup.Update(ref state);
            targetLookup.Update(ref state);
            abilityLookup.Update(ref state);
            projectileAbilityLookup.Update(ref state);
            obstacleLookup.Update(ref state);
            enemyDamageLookup.Update(ref state);
            activeForCollisionLookup.Update(ref state);

            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            CollisionDamageJob job = new CollisionDamageJob
            {
                ecb = ecb,
                projectileLookup = projectileLookup,
                healthLookup = healthLookup,
                targetLookup = targetLookup,
                abilityLookup = abilityLookup,
                projectileAbilityLookup = projectileAbilityLookup,
                obstacleLookup = obstacleLookup,
                enemyDamageLookup = enemyDamageLookup,
                activeForCollisionLookup = activeForCollisionLookup,
                deltaTime = SystemAPI.Time.fixedDeltaTime
            };

            SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

            JobHandle handle = job.Schedule(simulationSingleton, state.Dependency);
            state.Dependency = handle;
        }
    }
}