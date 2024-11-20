using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyMovementJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, ref EnemyComponent enemyComponent,
        in DynamicBuffer<NodeComponent> pathBuffer, in LocalTransform localTransform,
        ref PhysicsVelocity physicsVelocity, in Entity enemyEntity)
    {
        if (pathBuffer.Length == 0 || enemyComponent.moveTimer < enemyComponent.moveTimerTarget)
        {
            physicsVelocity.Linear = float3.zero;
            enemyComponent.moveTimer += deltaTime;

            if (enemyComponent.moveTimer >= enemyComponent.moveTimerTarget) enemyComponent.isFullySpawned = 1;

            return;
        }

        int nextPathIndex = math.min(enemyComponent.currentPathIndex + 1, pathBuffer.Length - 1);
        int2 nextPathPosition = pathBuffer[nextPathIndex].position;
        float3 currentPos3D = new float3(localTransform.Position.x, 0, localTransform.Position.z);
        float3 targetPos3D = new float3(nextPathPosition.x, 0, nextPathPosition.y);

        float3 direction = math.normalize(targetPos3D - currentPos3D);

        physicsVelocity.Linear = direction * enemyComponent.moveSpeed * deltaTime;

        int2 gridPosition = new int2((int)math.round(localTransform.Position.x),
            (int)math.round(localTransform.Position.z));

        if (!enemyComponent.gridPosition.Equals(gridPosition))
        {
            int2 oldPosition = enemyComponent.gridPosition;

            ecb.SetComponent(sortKey, enemyEntity, new GridEnemyPositionUpdateComponent
            {
                enemyEntity = enemyEntity,
                oldPosition = oldPosition,
                position = gridPosition,
                status = UpdateStatus.Move
            });
            ecb.AddComponent(sortKey, enemyEntity, new PositionChangedComponent());

            enemyComponent.gridPosition = gridPosition;
        }

        enemyComponent.position = localTransform.Position;

        if (math.lengthsq(currentPos3D - targetPos3D) < 0.01f) enemyComponent.currentPathIndex = nextPathIndex;
    }
}