using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public struct CollisionEventJob : ICollisionEventsJob
{
    [ReadOnly] public ComponentLookup<PlayerComponent> PlayerLookup;
    [ReadOnly] public ComponentLookup<EnemyComponent> EnemyLookup;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        bool isPlayerA = PlayerLookup.HasComponent(entityA);
        bool isEnemyA = EnemyLookup.HasComponent(entityA);
        bool isPlayerB = PlayerLookup.HasComponent(entityB);
        bool isEnemyB = EnemyLookup.HasComponent(entityB);

        if ((isPlayerA && isEnemyB) || (isPlayerB && isEnemyA))
        {
            Debug.Log("Player collided with enemy!");
        }
    }
}