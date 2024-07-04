using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemySpawnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly] public float3 spawnPosition;
    [ReadOnly] public double elapsedTime;

    public void Execute([ChunkIndexInQuery] int chunkIndex, ref EnemySpawnerComponent enemySpawner)
    {
        if (enemySpawner.nextSpawnTime < elapsedTime)
        {
            Entity spawnedEntity = ecb.Instantiate(chunkIndex, enemySpawner.prefab);

            ecb.SetComponent(chunkIndex, spawnedEntity, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            enemySpawner.nextSpawnTime = (float)elapsedTime + enemySpawner.spawnRate;
        }
    }
}
