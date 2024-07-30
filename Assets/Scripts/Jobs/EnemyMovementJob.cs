using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct EnemyMovementJob : IJobEntity
{
    [ReadOnly] public float deltaTime;

    void Execute(EnemyMovementAspect enemyMovement, in DynamicBuffer<Node> pathBuffer)
    {
        if (pathBuffer.Length == 0 || enemyMovement.enemy.ValueRO.currentPathIndex >= pathBuffer.Length)
        {
            return;
        }

        int2 targetPosition = pathBuffer[enemyMovement.enemy.ValueRO.currentPathIndex].position;
        int2 currentPosition = enemyMovement.enemy.ValueRO.position;
        float2 direction = (float2)(targetPosition - currentPosition);
        float distance = math.length(direction);

        if (distance < 0.01f)
        {
            enemyMovement.enemy.ValueRW.currentPathIndex++;
            if (enemyMovement.enemy.ValueRW.currentPathIndex >= pathBuffer.Length)
            {
                return;
            }
            targetPosition = pathBuffer[enemyMovement.enemy.ValueRW.currentPathIndex].position;
            direction = (float2)(targetPosition - currentPosition);
            distance = math.length(direction);
        }

        if (distance > 0)
        {
            float2 normalizedDirection = math.normalize(direction);
            float2 movement = normalizedDirection * enemyMovement.enemy.ValueRO.moveSpeed * deltaTime;

            if (math.length(movement) > distance)
            {
                movement = direction;
            }

            enemyMovement.enemy.ValueRW.position += (int2)math.round(movement);
            enemyMovement.physics.ValueRW.Linear = new float3(normalizedDirection.x, 0, normalizedDirection.y) * enemyMovement.enemy.ValueRO.moveSpeed * deltaTime;
        }
    }
}