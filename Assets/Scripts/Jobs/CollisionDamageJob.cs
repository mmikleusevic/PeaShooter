using Components;
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
    [ReadOnly] public ComponentLookup<AbilityComponent> abilityLookup;
    [ReadOnly] public ComponentLookup<ProjectileAbilityComponent> projectileAbilityLookup;
    [ReadOnly] public ComponentLookup<ObstacleComponent> obstacleLookup;
    [ReadOnly] public ComponentLookup<EnemyDamageComponent> enemyDamageLookup;
    [ReadOnly] public ComponentLookup<ActiveForCollisionComponent> activeForCollisionLookup;
    [ReadOnly] public float deltaTime;

    [BurstCompile]
    private bool HasHealth(Entity entity)
    {
        return healthLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasObstacle(Entity entity)
    {
        return obstacleLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasEnemy(Entity entity)
    {
        return enemyDamageLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasActiveForCollision(Entity entity)
    {
        return activeForCollisionLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasProjectile(Entity entity)
    {
        return projectileLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasTarget(Entity entity)
    {
        return targetLookup.HasComponent(entity);
    }

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        if ((HasObstacle(entityA) && HasHealth(entityB)) || (HasObstacle(entityB) && HasHealth(entityA))) return;

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

        EnemyDamageComponent enemyDamageComponent = enemyDamageLookup.GetRefRO(enemyEntity).ValueRO;

        playerHealthComponent.ValueRW.HitPoints -= enemyDamageComponent.damagePerSecond * deltaTime;

        if (playerHealthComponent.ValueRO.HitPoints == 0) ecb.RemoveComponent<PlayerAliveComponent>(playerEntity);
    }

    [BurstCompile]
    private void HandleProjectileCollision(Entity projectileEntity, Entity otherEntity)
    {
        RefRW<ProjectileComponent> projectileComponent = projectileLookup.GetRefRW(projectileEntity);

        if (projectileComponent.ValueRO.hasCollided == 1 || HasObstacle(otherEntity))
        {
            projectileComponent.ValueRW.hasCollided = 1;
            return;
        }

        if (HasTarget(projectileEntity))
        {
            TargetComponent targetComponent = targetLookup.GetRefRO(projectileEntity).ValueRO;

            if (targetComponent.enemyEntity == otherEntity && HasHealth(otherEntity))
            {
                projectileComponent.ValueRW.hasCollided = 1;

                RefRW<HealthComponent> enemyHealthComponent = healthLookup.GetRefRW(otherEntity);
                ProjectileAbilityComponent projectileAbilityComponent =
                    projectileAbilityLookup.GetRefRO(projectileEntity).ValueRO;
                AbilityComponent abilityComponent =
                    abilityLookup.GetRefRO(projectileAbilityComponent.parentEntity).ValueRO;

                enemyHealthComponent.ValueRW.HitPoints -= abilityComponent.damage;

                if (enemyHealthComponent.ValueRO.HitPoints <= 0)
                {
                    ecb.AddComponent(otherEntity, new EnemyDeadComponent());
                    ecb.SetComponent(otherEntity, new GridEnemyPositionUpdateComponent
                    {
                        entity = otherEntity,
                        oldPosition = targetComponent.enemy.gridPosition,
                        status = UpdateStatus.Remove,
                        position = targetComponent.enemy.gridPosition
                    });
                    ecb.AddComponent(otherEntity, new HasChangedPositionComponent());
                    ecb.AddComponent(otherEntity, new DestroyComponent());
                }
            }
        }
    }
}