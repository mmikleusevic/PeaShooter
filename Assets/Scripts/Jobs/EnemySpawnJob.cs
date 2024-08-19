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
    [ReadOnly] public GridComponent grid;

    private void Execute(ref EnemySpawnerComponent enemySpawner, ref RandomDataComponent randomData)
    {
        if (enemySpawner.nextSpawnTime < elapsedTime)
        {
            Entity spawnedEntity = ecb.Instantiate(enemySpawner.prefab);

            int2 newPosition = default;

            do
            {
                newPosition = randomData.nextPosition;
            }
            while (!grid.gridNodes[newPosition]);

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = new float3(newPosition.x, 0, newPosition.y),
                Rotation = quaternion.identity,
                Scale = enemySpawner.scale
            });

            ecb.AddComponent(spawnedEntity, new EnemyComponent
            {
                moveSpeed = enemySpawner.moveSpeed,
                position = newPosition,
                currentPathIndex = 0,
            });

            ecb.AddBuffer<Node>(spawnedEntity);

            enemySpawner.nextSpawnTime = (float)elapsedTime + enemySpawner.spawnRate;
        }
    }
}
