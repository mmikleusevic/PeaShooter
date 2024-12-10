using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;


namespace Jobs
{
    [BurstCompile]
    public partial struct ObstacleSpawnJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        public NativeHashMap<int2, byte> gridNodes;

        [ReadOnly] public uint seed;

        private void Execute(in ObstacleSpawnerComponent obstacleSpawnerComponent,
            ref RandomDataComponent randomDataComponent, in Entity obstacleSpawnerEntity)
        {
            randomDataComponent.seed = new Random(seed);

            for (int i = 0; i < obstacleSpawnerComponent.numberToSpawn; i++)
            {
                int randomValue = randomDataComponent.seed.NextInt(0, 2);

                Entity spawnedEntity;

                if (randomValue == 0)
                {
                    spawnedEntity = ecb.Instantiate(obstacleSpawnerComponent.prefab1Entity);
                }
                else
                {
                    spawnedEntity = ecb.Instantiate(obstacleSpawnerComponent.prefab2Entity);
                }

                ecb.SetName(spawnedEntity, "Obstacle");

                int2 newPosition = randomDataComponent.GetRandomPosition(gridNodes);

                ecb.SetComponent(spawnedEntity, new LocalTransform
                {
                    Position = new float3(newPosition.x, 0, newPosition.y),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                ecb.AddComponent(spawnedEntity, new ObstacleComponent());

                gridNodes[newPosition] = 0;
            }

            ecb.DestroyEntity(obstacleSpawnerEntity);
        }
    }
}