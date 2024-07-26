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
    [ReadOnly] public NativeList<ObstacleComponent> obstacles;

    public void Execute(ref EnemySpawnerComponent enemySpawner, ref RandomDataComponent randomData)
    {
        if (enemySpawner.nextSpawnTime < elapsedTime)
        {
            Entity spawnedEntity = ecb.Instantiate(enemySpawner.prefab);

            float3 newPosition = default;
            CheckObstacles.GetValidPosition(obstacles, ref randomData, enemySpawner.scale, ref newPosition);

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = newPosition,
                Rotation = quaternion.identity,
                Scale = enemySpawner.scale
            });

            ecb.AddBuffer<Node>(spawnedEntity);

            enemySpawner.nextSpawnTime = (float)elapsedTime + enemySpawner.spawnRate;
        }
    }
}
