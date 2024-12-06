#region

using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

#endregion

namespace Jobs
{
    [BurstCompile]
    public partial struct EnemySpawnJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public NativeHashMap<int2, byte> gridNodes;

        [ReadOnly] public double elapsedTime;
        [ReadOnly] public uint seed;

        private void Execute([EntityIndexInQuery] int sortKey, ref EnemySpawnerComponent enemySpawnerComponent,
            ref RandomDataComponent randomDataComponent, in Entity enemySpawnerEntity)
        {
            if (enemySpawnerComponent.startTime == 0) enemySpawnerComponent.startTime = elapsedTime;

            double localElapsedTime = elapsedTime - enemySpawnerComponent.startTime;

            if (enemySpawnerComponent.nextSpawnTime >= localElapsedTime) return;

            randomDataComponent.seed = new Random((uint)(seed + sortKey));

            int2 newPosition = randomDataComponent.GetRandomPosition(gridNodes);

            float3 position = new float3(newPosition.x, 0, newPosition.y);

            Entity spawnedEntity = ecb.Instantiate(sortKey, enemySpawnerComponent.prefabEntity);

            ecb.SetName(sortKey, spawnedEntity, "Enemy");

            ecb.SetComponent(sortKey, spawnedEntity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = enemySpawnerComponent.scale
            });

            ecb.AddComponent(sortKey, spawnedEntity, new EnemyComponent
            {
                moveSpeed = enemySpawnerComponent.moveSpeed,
                gridPosition = newPosition,
                position = position,
                isFullySpawned = 0,
                currentPathIndex = 0,
                moveTimerTarget = enemySpawnerComponent.enemyMoveTimerTarget
            });

            ecb.AddBuffer<NodeComponent>(sortKey, spawnedEntity);

            if (localElapsedTime >= enemySpawnerComponent.destroySpawnerTimerTarget)
                ecb.DestroyEntity(sortKey, enemySpawnerEntity);

            enemySpawnerComponent.nextSpawnTime += enemySpawnerComponent.spawnRate;
        }
    }
}