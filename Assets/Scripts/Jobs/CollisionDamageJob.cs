using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
public struct CollisionDamageJob : ICollisionEventsJob
{
    public EntityCommandBuffer ecb;
    public ComponentLookup<ProjectileComponent> projectileComponentLookup;
    public ComponentLookup<HealthComponent> healthComponentLookup;
    public ComponentLookup<BarrierComponent> barrierComponentLookup;

    [ReadOnly] public ComponentLookup<TargetComponent> targetComponentLookup;
    [ReadOnly] public ComponentLookup<AbilityComponent> abilityComponentLookup;
    [ReadOnly] public ComponentLookup<ProjectileAbilityComponent> projectileAbilityComponentLookup;
    [ReadOnly] public ComponentLookup<ObstacleComponent> obstacleComponentLookup;
    [ReadOnly] public ComponentLookup<EnemyDamageComponent> enemyDamageComponentLookup;
    [ReadOnly] public ComponentLookup<CollisionActiveComponent> activeForCollisionComponentLookup;
    [ReadOnly] public float deltaTime;

    [BurstCompile]
    private bool HasHealth(Entity entity)
    {
        return healthComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasObstacle(Entity entity)
    {
        return obstacleComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasEnemy(Entity entity)
    {
        return enemyDamageComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasActiveForCollision(Entity entity)
    {
        return activeForCollisionComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasProjectile(Entity entity)
    {
        return projectileComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasTarget(Entity entity)
    {
        return targetComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasProjectileAbility(Entity entity)
    {
        return projectileAbilityComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasAbility(Entity entity)
    {
        return abilityComponentLookup.HasComponent(entity);
    }

    [BurstCompile]
    private bool HasBarrier(Entity entity)
    {
        return barrierComponentLookup.HasComponent(entity);
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

        EnemyDamageComponent enemyDamageComponent = enemyDamageComponentLookup.GetRefRO(enemyEntity).ValueRO;

        float damage = enemyDamageComponent.damagePerSecond * deltaTime;

        if (HasBarrier(playerEntity))
        {
            RefRW<BarrierComponent> barrierComponentRW = barrierComponentLookup.GetRefRW(playerEntity);
            float damageToBarrier = math.min(barrierComponentRW.ValueRW.BarrierValue, damage);

            barrierComponentRW.ValueRW.BarrierValue -= damageToBarrier;
            damage -= damageToBarrier;
        }

        float damageToHealth = damage;

        RefRW<HealthComponent> playerHealthComponent = healthComponentLookup.GetRefRW(playerEntity);

        if (playerHealthComponent.ValueRO.IsDead) return;

        playerHealthComponent.ValueRW.HitPoints -= damageToHealth;

        if (playerHealthComponent.ValueRO.IsDead) ecb.RemoveComponent<PlayerAliveComponent>(playerEntity);
    }

    [BurstCompile]
    private void HandleProjectileCollision(Entity projectileEntity, Entity otherEntity)
    {
        RefRW<ProjectileComponent> projectileComponent = projectileComponentLookup.GetRefRW(projectileEntity);

        if (projectileComponent.ValueRO.hasCollided == 1 || HasObstacle(otherEntity))
        {
            projectileComponent.ValueRW.hasCollided = 1;
            return;
        }

        if (HasTarget(projectileEntity))
        {
            TargetComponent targetComponent = targetComponentLookup.GetRefRO(projectileEntity).ValueRO;

            if (targetComponent.enemyEntity == otherEntity && HasHealth(otherEntity))
            {
                projectileComponent.ValueRW.hasCollided = 1;

                RefRW<HealthComponent> enemyHealthComponent = healthComponentLookup.GetRefRW(otherEntity);

                if (!HasProjectileAbility(projectileEntity)) return;

                ProjectileAbilityComponent projectileAbilityComponent =
                    projectileAbilityComponentLookup.GetRefRO(projectileEntity).ValueRO;

                if (!HasAbility(projectileAbilityComponent.parentEntity)) return;

                AbilityComponent abilityComponent =
                    abilityComponentLookup.GetRefRO(projectileAbilityComponent.parentEntity).ValueRO;

                float damage = abilityComponent.damage;

                if (HasBarrier(otherEntity))
                {
                    RefRW<BarrierComponent> barrierComponentRW = barrierComponentLookup.GetRefRW(otherEntity);
                    float damageToBarrier = math.min(barrierComponentRW.ValueRW.BarrierValue, abilityComponent.damage);

                    barrierComponentRW.ValueRW.BarrierValue -= damageToBarrier;
                    damage -= damageToBarrier;
                }

                float damageToHealth = damage;

                enemyHealthComponent.ValueRW.HitPoints -= damageToHealth;

                if (enemyHealthComponent.ValueRO.IsDead)
                {
                    ecb.AddComponent(otherEntity, new EnemyDeadComponent());
                    ecb.SetComponent(otherEntity, new GridEnemyPositionUpdateComponent
                    {
                        enemyEntity = otherEntity,
                        oldPosition = targetComponent.enemyComponent.gridPosition,
                        status = UpdateStatus.Remove,
                        position = targetComponent.enemyComponent.gridPosition
                    });
                    ecb.AddComponent(otherEntity, new PositionChangedComponent());
                    ecb.AddComponent(otherEntity, new DestroyComponent());
                }
            }
        }
    }
}