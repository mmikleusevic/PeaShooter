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

    [ReadOnly] public double elapsedTime;
    [ReadOnly] public GridComponent grid;
    [ReadOnly] public uint seed;

    private void Execute([ChunkIndexInQuery] int sortKey, ref EnemySpawnerComponent enemySpawner, ref RandomDataComponent randomData)
    {
        if (enemySpawner.nextSpawnTime >= elapsedTime) return;

        randomData.seed = new Random(seed);

        Entity spawnedEntity = ecb.Instantiate(sortKey, enemySpawner.prefab);

        int2 newPosition = default;

        do
        {
            newPosition = randomData.nextPosition;
        }
        while (!grid.gridNodes[newPosition]);

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
            isFullySpawned = false,
            currentPathIndex = 0,
            moveTimerTarget = enemySpawner.enemyMoveTimerTarget
        });

        ecb.AddBuffer<NodeComponent>(sortKey, spawnedEntity);

        enemySpawner.nextSpawnTime += enemySpawner.spawnRate;
    }
}