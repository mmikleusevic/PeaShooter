using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct EnemySpawnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public NativeHashMap<int2, byte> gridNodes;

    [ReadOnly] public double elapsedTime;
    [ReadOnly] public uint seed;

    private void Execute([EntityIndexInQuery] int sortKey, ref EnemySpawnerComponent enemySpawner,
        ref RandomDataComponent randomData, in Entity entity)
    {
        if (enemySpawner.startTime == 0) enemySpawner.startTime = elapsedTime;

        double localElapsedTime = elapsedTime - enemySpawner.startTime;

        if (enemySpawner.nextSpawnTime >= localElapsedTime) return;

        randomData.seed = new Random((uint)(seed + sortKey));

        Entity spawnedEntity = ecb.Instantiate(sortKey, enemySpawner.prefab);

        int2 newPosition = default;

        do
        {
            newPosition = randomData.nextPosition;
        } while (gridNodes[newPosition] == 0);

        float3 position = new float3(newPosition.x, 0, newPosition.y);

        ecb.SetComponent(sortKey, spawnedEntity, new LocalTransform
        {
            Position = position,
            Rotation = quaternion.identity,
            Scale = enemySpawner.scale
        });

        ecb.AddComponent(sortKey, spawnedEntity, new EnemyComponent
        {
            moveSpeed = enemySpawner.moveSpeed,
            gridPosition = newPosition,
            position = position,
            isFullySpawned = 0,
            currentPathIndex = 0,
            moveTimerTarget = enemySpawner.enemyMoveTimerTarget
        });

        ecb.AddBuffer<NodeComponent>(sortKey, spawnedEntity);

        if (localElapsedTime >= enemySpawner.destroySpawnerTimerTarget) ecb.DestroyEntity(sortKey, entity);

        enemySpawner.nextSpawnTime += enemySpawner.spawnRate;
    }
}