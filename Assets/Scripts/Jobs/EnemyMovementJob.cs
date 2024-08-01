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
        if (pathBuffer.Length == 0)
        {
            enemyMovement.physics.ValueRW.Linear = float3.zero;
            return;
        }

        float3 currentPos3D = new float3(enemyMovement.transform.ValueRO.Position.x, 0, enemyMovement.transform.ValueRO.Position.z);
        int nextPathIndex = math.min(enemyMovement.enemy.ValueRO.currentPathIndex + 1, pathBuffer.Length - 1);
        float3 targetPos3D = new float3(pathBuffer[nextPathIndex].position.x, 0, pathBuffer[nextPathIndex].position.y);

        float3 direction = math.normalize(targetPos3D - currentPos3D);

        enemyMovement.physics.ValueRW.Linear = direction * enemyMovement.enemy.ValueRO.moveSpeed * deltaTime;
        enemyMovement.transform.ValueRW.Position.y = 0;
        enemyMovement.enemy.ValueRW.position = new int2((int)math.round(enemyMovement.transform.ValueRO.Position.x), (int)math.round(enemyMovement.transform.ValueRO.Position.z));

        if (math.lengthsq(currentPos3D - targetPos3D) < 0.01f)
        {
            enemyMovement.enemy.ValueRW.currentPathIndex = nextPathIndex;
        }
    }
}