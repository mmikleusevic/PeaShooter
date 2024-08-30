using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public struct CollisionDamageJob : ICollisionEventsJob
{
    public ComponentLookup<ProjectileComponent> projectileLookup;
    public ComponentLookup<HealthComponent> healthLookup;
    public EntityCommandBuffer ecb;

    [ReadOnly] public ComponentLookup<EnemyDamageComponent> enemyDamageLookup;
    [ReadOnly] public ComponentLookup<ActiveForCollisionComponent> activeForCollisionLookup;
    [ReadOnly] public float deltaTime;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        bool playerCollision = false;

        if (healthLookup.HasComponent(entityA) && enemyDamageLookup.HasComponent(entityB) && activeForCollisionLookup.HasComponent(entityB))
        {
            HandlePlayerCollision(playerCollision, entityA, entityB);
        }
        else if (healthLookup.HasComponent(entityB) && enemyDamageLookup.HasComponent(entityA) && activeForCollisionLookup.HasComponent(entityA))
        {
            HandlePlayerCollision(playerCollision, entityB, entityA);
        }

        if (playerCollision) return;

        if (projectileLookup.HasComponent(entityA) && healthLookup.HasComponent(entityB))
        {
            HandleProjectileCollision(entityA, entityB);
        }
        else if (projectileLookup.HasComponent(entityB) && healthLookup.HasComponent(entityA))
        {
            HandleProjectileCollision(entityB, entityA);
        }
    }

    private void HandlePlayerCollision(bool collision, Entity playerEntity, Entity enemyEntity)
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

    private void HandleProjectileCollision(Entity projectileEntity, Entity enemyEntity)
    {
        RefRW<HealthComponent> enemyHealthComponent = healthLookup.GetRefRW(enemyEntity);
        RefRW<ProjectileComponent> projectileComponent = projectileLookup.GetRefRW(projectileEntity);

        projectileComponent.ValueRW.hasCollidedWithEnemy = true;

        enemyHealthComponent.ValueRW.HitPoints -= projectileComponent.ValueRO.damage;

        if (enemyHealthComponent.ValueRW.HitPoints == 0)
        {
            ecb.DestroyEntity(enemyEntity);
        }
    }
}