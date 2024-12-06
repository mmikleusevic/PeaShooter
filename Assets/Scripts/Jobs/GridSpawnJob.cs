#region

using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

#endregion

namespace Jobs
{
    [BurstCompile]
    public partial struct GridSpawnJob : IJobEntity
    {
        public EntityCommandBuffer ecb;

        private void Execute(in GridSpawnerComponent gridSpawnerComponent, in Entity gridSpawnerEntity)
        {
            int numberOfPositionPairs = math.square(gridSpawnerComponent.size.x + gridSpawnerComponent.size.y + 1);

            GridComponent gridComponent = new GridComponent
            {
                gridNodes = new NativeHashMap<int2, byte>(numberOfPositionPairs, Allocator.Persistent),
                enemyPositions =
                    new NativeParallelMultiHashMap<int2, Entity>(numberOfPositionPairs, Allocator.Persistent)
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

            Entity gridSpawnedEntity = ecb.Instantiate(gridSpawnerComponent.prefabEntity);

            ecb.SetName(gridSpawnedEntity, "Grid");

            ecb.AddComponent(gridSpawnedEntity, gridComponent);
            ecb.DestroyEntity(gridSpawnerEntity);
        }
    }
}