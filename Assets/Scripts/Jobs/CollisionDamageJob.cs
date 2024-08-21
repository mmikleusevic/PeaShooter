using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public struct CollisionDamageJob : ICollisionEventsJob
{
    public ComponentLookup<PlayerHealthComponent> playerHealthLookup;
    [ReadOnly] public ComponentLookup<EnemyDamageComponent> enemyDamageLookup;
    [ReadOnly] public float deltaTime;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        Entity playerEntity = default;
        Entity enemyEntity = default;

        bool isCollision = false;

        if (playerHealthLookup.HasComponent(entityA) && enemyDamageLookup.HasComponent(entityB))
        {
            playerEntity = entityA;
            enemyEntity = entityB;
            isCollision = true;
        }
        else if (playerHealthLookup.HasComponent(entityB) && enemyDamageLookup.HasComponent(entityA))
        {
            playerEntity = entityB;
            enemyEntity = entityA;
            isCollision = true;
        }

        if (!isCollision) return;

        RefRW<PlayerHealthComponent> playerHealthComponent = playerHealthLookup.GetRefRW(playerEntity);

        if (playerHealthComponent.ValueRO.IsDead) return;

        RefRO<EnemyDamageComponent> enemyDamageComponent = enemyDamageLookup.GetRefRO(enemyEntity);

        playerHealthComponent.ValueRW.HitPoints -= enemyDamageComponent.ValueRO.damagePerSecond * deltaTime;
    }
}