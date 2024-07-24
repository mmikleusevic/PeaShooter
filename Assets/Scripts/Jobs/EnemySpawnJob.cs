using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemySpawnJob : IJobEntity
{
    public EntityCommandBuffer ecb;

    [ReadOnly] public double elapsedTime;

    public void Execute([ChunkIndexInQuery] int chunkIndex, ref EnemySpawnerComponent enemySpawner, ref RandomDataComponent randomData)
    {
        if (enemySpawner.nextSpawnTime < elapsedTime)
        {
            Entity spawnedEntity = ecb.Instantiate(enemySpawner.prefab);

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = randomData.nextPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            ecb.AddBuffer<Node>(spawnedEntity);

            enemySpawner.nextSpawnTime = (float)elapsedTime + enemySpawner.spawnRate;
        }
    }
}
