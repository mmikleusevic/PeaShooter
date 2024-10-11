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

    void Execute([ChunkIndexInQuery] int sortKey, ref EnemyComponent enemy, in DynamicBuffer<NodeComponent> pathBuffer, in LocalTransform transform,
        ref PhysicsVelocity velocity, in Entity entity)
    {
        if (pathBuffer.Length == 0 || enemy.moveTimer < enemy.moveTimerTarget)
        {
            velocity.Linear = float3.zero;
            enemy.moveTimer += deltaTime;

            if (enemy.moveTimer >= enemy.moveTimerTarget) enemy.isFullySpawned = 1;

            return;
        }

        int nextPathIndex = math.min(enemy.currentPathIndex + 1, pathBuffer.Length - 1);
        int2 nextPathPosition = pathBuffer[nextPathIndex].position;
        float3 currentPos3D = new float3(transform.Position.x, 0, transform.Position.z);
        float3 targetPos3D = new float3(nextPathPosition.x, 0, nextPathPosition.y);

        float3 direction = math.normalize(targetPos3D - currentPos3D);

        velocity.Linear = direction * enemy.moveSpeed * deltaTime;

        int2 gridPosition = new int2((int)math.round(transform.Position.x), (int)math.round(transform.Position.z));

        if (!enemy.gridPosition.Equals(gridPosition))
        {
            int2 oldPosition = enemy.gridPosition;

            ecb.AddComponent(sortKey, entity, new GridEnemyPositionUpdateComponent
            {
                entity = entity,
                oldPosition = oldPosition,
                position = gridPosition,
                status = UpdateStatus.Move
            });

            enemy.gridPosition = gridPosition;
        }

        enemy.position = transform.Position;

        if (math.lengthsq(currentPos3D - targetPos3D) < 0.01f)
        {
            enemy.currentPathIndex = nextPathIndex;
        }
    }
}