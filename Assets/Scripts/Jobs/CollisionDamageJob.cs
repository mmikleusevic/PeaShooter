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

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        bool playerEnemyCollision = false;

        if (healthLookup.HasComponent(entityA) && enemyDamageLookup.HasComponent(entityB) && activeForCollisionLookup.HasComponent(entityB))
        {
            HandlePlayerCollision(ref playerEnemyCollision, entityA, entityB);
        }
        else if (healthLookup.HasComponent(entityB) && enemyDamageLookup.HasComponent(entityA) && activeForCollisionLookup.HasComponent(entityA))
        {
            HandlePlayerCollision(ref playerEnemyCollision, entityB, entityA);
        }

        if (playerEnemyCollision) return;

        if (projectileLookup.HasComponent(entityA))
        {
            HandleProjectileCollision(entityA, entityB);
        }
        else if (projectileLookup.HasComponent(entityB))
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
            ecb.AddComponent<PlayerDeadComponent>(playerEntity);
        }
    }

    [BurstCompile]
    private void HandleProjectileCollision(Entity projectileEntity, Entity otherEntity)
    {
        RefRW<ProjectileComponent> projectileComponent = projectileLookup.GetRefRW(projectileEntity);

        if (projectileComponent.ValueRW.hasCollided) return;

        projectileComponent.ValueRW.hasCollided = true;

        if (obstacleLookup.HasComponent(otherEntity)) return;

        if (targetLookup.HasComponent(projectileEntity))
        {
            RefRO<TargetComponent> targetComponent = targetLookup.GetRefRO(projectileEntity);

            if (targetComponent.ValueRO.enemyEntity == otherEntity && healthLookup.HasComponent(otherEntity))
            {
                RefRW<HealthComponent> enemyHealthComponent = healthLookup.GetRefRW(otherEntity);

                enemyHealthComponent.ValueRW.HitPoints -= projectileComponent.ValueRO.damage;

                if (enemyHealthComponent.ValueRW.HitPoints == 0)
                {
                    ecb.DestroyEntity(otherEntity);
                }
            }
        }
    }
}