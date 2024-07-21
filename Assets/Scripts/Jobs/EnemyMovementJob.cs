using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct EnemyMovementJob : IJobEntity
{
    [ReadOnly] public float3 PlayerPosition;
    [ReadOnly] public float DeltaTime;

    void Execute(EnemyMovementAspect enemyMovement)
    {
        float3 enemyPosition = enemyMovement.transform.ValueRO.Position;
        float3 direction = math.normalize(PlayerPosition - enemyPosition);
        enemyMovement.physics.ValueRW.Linear = direction * enemyMovement.enemyController.ValueRO.moveSpeed * DeltaTime;
    }
}
