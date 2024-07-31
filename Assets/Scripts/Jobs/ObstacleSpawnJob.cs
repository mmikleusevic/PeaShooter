using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ObstacleSpawnJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public GridComponent grid;

    public void Execute(in ObstacleSpawnerComponent spawner, ref RandomDataComponent randomData)
    {
        for (int i = 0; i < spawner.numberToSpawn; i++)
        {
            Entity spawnedEntity = ecb.Instantiate(spawner.prefab);

            int2 newPosition = default;

            do
            {
                newPosition = randomData.nextPosition;
            }
            while (!IsValidPosition(newPosition, grid));

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = new float3(newPosition.x, 0, newPosition.y),
                Rotation = quaternion.identity,
                Scale = 1f,
            });

            grid.gridNodes[newPosition] = false;
        }
    }

    private bool IsValidPosition(int2 position, GridComponent grid)
    {
        return grid.gridNodes[position] && position.x != 0 && position.y != 0;
    }
}