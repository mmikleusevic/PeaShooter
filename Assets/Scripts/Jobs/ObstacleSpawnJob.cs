using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ObstacleSpawnJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public NativeList<float3> positionsOccupied;

    public void Execute(ref ObstacleSpawnerComponent spawner, ref RandomDataComponent randomData)
    {
        for (int i = 0; i < spawner.numberToSpawn; i++)
        {
            Entity spawnedEntity = ecb.Instantiate(spawner.prefab);
            float3 newPosition = default;

            CheckObstacles.GetValidPosition(positionsOccupied, ref randomData, spawner.scale, ref newPosition);

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = newPosition,
                Rotation = quaternion.identity,
                Scale = spawner.scale,
            });

            positionsOccupied.Add(newPosition);
        }
    }
}