using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct ObstacleSpawnJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public NativeHashMap<int2, byte> gridNodes;

    [ReadOnly] public uint seed;

    private void Execute(in ObstacleSpawnerComponent spawner, ref RandomDataComponent randomData, in Entity entity)
    {
        randomData.seed = new Random(seed);

        for (int i = 0; i < spawner.numberToSpawn; i++)
        {
            Entity spawnedEntity = ecb.Instantiate(spawner.prefab);

            int2 newPosition = default;

            do
            {
                newPosition = randomData.nextPosition;
            } while (!IsValidPosition(newPosition));

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = new float3(newPosition.x, 0, newPosition.y),
                Rotation = quaternion.identity,
                Scale = 1f
            });

            ecb.AddComponent(spawnedEntity, new ObstacleComponent());

            gridNodes[newPosition] = 0;
        }

        ecb.DestroyEntity(entity);
    }

    [BurstCompile]
    private bool IsValidPosition(int2 position)
    {
        return gridNodes[position] == 1 && position.x != 0 && position.y != 0;
    }
}