using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public struct CollisionDamageJob : ICollisionEventsJob
{
    public EntityCommandBuffer ecb;
    public ComponentLookup<ProjectileComponent> projectileLookup;
    public ComponentLookup<HealthComponent> healthLookup;

    [ReadOnly] public ComponentLookup<TargetComponent> targetLookup;
    [ReadOnly] public ComponentLookup<ObstacleComponent> obstacleLookup;
    [ReadOnly] public ComponentLookup<EnemyDamageComponent> enemyDamageLookup;
    [ReadOnly] public ComponentLookup<ActiveForCollisionComponent> activeForCollisionLookup;
    [ReadOnly] public float deltaTime;

    private bool HasHealth(Entity entity) => healthLookup.HasComponent(entity);
    private bool HasObstacle(Entity entity) => obstacleLookup.HasComponent(entity);
    private bool HasEnemy(Entity entity) => enemyDamageLookup.HasComponent(entity);
    private bool HasActiveForCollision(Entity entity) => activeForCollisionLookup.HasComponent(entity);
    private bool HasProjectile(Entity entity) => projectileLookup.HasComponent(entity);

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        if (HasObstacle(entityA) && HasHealth(entityB) || HasObstacle(entityB) && HasHealth(entityA))
        {
            return;
        }

        bool playerEnemyCollision = false;

        if (HasHealth(entityA) && HasEnemy(entityB) && HasActiveForCollision(entityB))
        {
            HandlePlayerCollision(ref playerEnemyCollision, entityA, entityB);
        }
        else if (HasHealth(entityB) && HasEnemy(entityA) && HasActiveForCollision(entityA))
        {
            HandlePlayerCollision(ref playerEnemyCollision, entityB, entityA);
        }

        if (playerEnemyCollision) return;

        if (HasProjectile(entityA))
        {
            HandleProjectileCollision(entityA, entityB);
        }
        else if (HasProjectile(entityB))
        {
            HandleProjectileCollision(entityB, entityA);
        }
    }

    [BurstCompile]
    private void HandlePlayerCollision(ref bool collision, Entity playerEntity, Entity enemyEntity)
    {
        collision = true;

        RefRW<HealthComponent> playerHealthComponent = healthLookup.GetRefRW(playerEntity);

        if (playerHealthComponent.ValueRO.HitPoints == 0) return;

        RefRO<EnemyDamageComponent> enemyDamageComponent = enemyDamageLookup.GetRefRO(enemyEntity);

        playerHealthComponent.ValueRW.HitPoints -= enemyDamageComponent.ValueRO.damagePerSecond * deltaTime;

        if (playerHealthComponent.ValueRO.HitPoints == 0)
        {
            ecb.RemoveComponent<PlayerAliveComponent>(playerEntity);
        }
    }

    [BurstCompile]
    private void HandleProjectileCollision(Entity projectileEntity, Entity otherEntity)
    {
        RefRW<ProjectileComponent> projectileComponent = projectileLookup.GetRefRW(projectileEntity);

        if (projectileComponent.ValueRW.hasCollided == 1) return;

        if (obstacleLookup.HasComponent(otherEntity))
        {
            projectileComponent.ValueRW.hasCollided = 1;
            return;
        }

        if (targetLookup.HasComponent(projectileEntity))
        {
            RefRO<TargetComponent> targetComponent = targetLookup.GetRefRO(projectileEntity);

            if (targetComponent.ValueRO.enemyEntity == otherEntity && healthLookup.HasComponent(otherEntity))
            {
                projectileComponent.ValueRW.hasCollided = 1;

                RefRW<HealthComponent> enemyHealthComponent = healthLookup.GetRefRW(otherEntity);

                enemyHealthComponent.ValueRW.HitPoints -= projectileComponent.ValueRO.damage;

                if (enemyHealthComponent.ValueRW.HitPoints == 0)
                {
                    ecb.AddComponent(otherEntity, new EnemyDeadComponent());
                    ecb.AddComponent(otherEntity, new DestroyComponent());
                }
            }
        }
    }
}