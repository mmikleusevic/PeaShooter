using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct GridSpawnJob : IJobEntity
{
    public EntityCommandBuffer ecb;

    private void Execute(in GridSpawnerComponent gridSpawnerComponent, in Entity gridSpawner)
    {
        int numberOfPositionPairs = math.square(gridSpawnerComponent.size.x + gridSpawnerComponent.size.y + 1);

        GridComponent gridComponent = new GridComponent
        {
            gridNodes = new NativeHashMap<int2, byte>(numberOfPositionPairs, Allocator.Persistent),
            enemyPositions = new NativeHashMap<int2, NativeList<Entity>>(numberOfPositionPairs, Allocator.Persistent)
        };

        for (int i = -gridSpawnerComponent.size.x; i <= gridSpawnerComponent.size.x; i++)
        {
            for (int j = -gridSpawnerComponent.size.y; j <= gridSpawnerComponent.size.y; j++)
            {
                int2 position = new int2(i, j);
                gridComponent.gridNodes[position] = 1;
            }
        }

        gridComponent.size = gridSpawnerComponent.size;

        Entity spawnedEntity = ecb.Instantiate(gridSpawnerComponent.prefab);

        ecb.AddComponent(spawnedEntity, gridComponent);
        ecb.DestroyEntity(gridSpawner);
    }
}