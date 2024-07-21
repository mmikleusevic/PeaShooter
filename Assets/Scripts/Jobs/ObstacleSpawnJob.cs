using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ObstacleSpawnJob : IJobFor
{
    public EntityCommandBuffer commandBuffer;
    public RandomDataComponent randomData;

    [ReadOnly] public Entity prefabToSpawn;

    public void Execute(int index)
    {
        Entity spawnedEntity = commandBuffer.Instantiate(prefabToSpawn);

        commandBuffer.SetComponent(spawnedEntity, new LocalTransform
        {
            Position = randomData.nextPosition,
            Rotation = quaternion.identity,
            Scale = 1f,
        });
    }
}