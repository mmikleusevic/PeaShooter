using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyMovementJob : IJobEntity
{
    [ReadOnly] public float deltaTime;

    void Execute(ref EnemyComponent enemy, in DynamicBuffer<NodeComponent> pathBuffer, in LocalTransform transform, ref PhysicsVelocity velocity)
    {
        if (pathBuffer.Length == 0 || enemy.moveTimer < enemy.moveTimerTarget)
        {
            velocity.Linear = float3.zero;
            enemy.moveTimer += deltaTime;

            if (enemy.moveTimer >= enemy.moveTimerTarget) enemy.isFullySpawned = true;

            return;
        }

        float3 currentPos3D = new float3(transform.Position.x, 0, transform.Position.z);
        int nextPathIndex = math.min(enemy.currentPathIndex + 1, pathBuffer.Length - 1);
        float3 targetPos3D = new float3(pathBuffer[nextPathIndex].position.x, 0, pathBuffer[nextPathIndex].position.y);

        float3 direction = math.normalize(targetPos3D - currentPos3D);

        velocity.Linear = direction * enemy.moveSpeed * deltaTime;
        enemy.gridPosition = new int2((int)math.round(transform.Position.x), (int)math.round(transform.Position.z));
        enemy.position = transform.Position;

        if (math.lengthsq(currentPos3D - targetPos3D) < 0.01f)
        {
            enemy.currentPathIndex = nextPathIndex;
        }
    }
}