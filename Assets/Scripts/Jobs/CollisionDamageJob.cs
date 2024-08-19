using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public struct CollisionDamageJob : ICollisionEventsJob
{
    public ComponentLookup<PlayerHealthComponent> playerHealthLookup;
    [ReadOnly] public ComponentLookup<EnemyDamageComponent> enemyDamageLookup;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        Entity playerEntity = default;
        Entity enemyEntity = default;

        bool isPlayerA = playerHealthLookup.HasComponent(entityA);
        bool isEnemyA = enemyDamageLookup.HasComponent(entityA);
        bool isPlayerB = playerHealthLookup.HasComponent(entityB);
        bool isEnemyB = enemyDamageLookup.HasComponent(entityB);

        bool haveCollided = false;

        if (isPlayerA && isEnemyB)
        {
            playerEntity = entityA;
            enemyEntity = entityB;
            haveCollided = true;
        }
        else if (isPlayerB && isEnemyA)
        {
            playerEntity = entityB;
            enemyEntity = entityA;
            haveCollided = true;
        }

        if (!haveCollided) return;

        RefRW<PlayerHealthComponent> playerHealthComponent = playerHealthLookup.GetRefRW(playerEntity);

        if (playerHealthComponent.ValueRO.isDead) return;

        RefRO<EnemyDamageComponent> enemyDamageComponent = enemyDamageLookup.GetRefRO(enemyEntity);

        playerHealthComponent.ValueRW.HitPoints -= enemyDamageComponent.ValueRO.damage;

        if (playerHealthComponent.ValueRO.HitPoints <= 0)
        {
            playerHealthComponent.ValueRW.isDead = true;
            Debug.Log("Player has died");
        }
    }
}