using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial struct GridSpawnerSystem : ISystem
{
    private GridComponent grid;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity gridSpawnerEntity = SystemAPI.GetSingletonEntity<GridSpawnerComponent>();

        RefRO<GridSpawnerComponent> gridSpawner = SystemAPI.GetComponentRO<GridSpawnerComponent>(gridSpawnerEntity);

        Entity spawnedEntity = state.EntityManager.Instantiate(gridSpawner.ValueRO.prefab);

        grid = new GridComponent
        {
            gridNodes = new NativeHashMap<int2, bool>(math.square(gridSpawner.ValueRO.size.x + gridSpawner.ValueRO.size.y + 1), Allocator.Persistent)
        };

        for (int i = -gridSpawner.ValueRO.size.x; i <= gridSpawner.ValueRO.size.x; i++)
        {
            for (int j = -gridSpawner.ValueRO.size.y; j <= gridSpawner.ValueRO.size.y; j++)
            {
                int2 position = new int2(i, j);
                grid.gridNodes[position] = true;
            }
        }

        grid.size = gridSpawner.ValueRO.size;

        state.EntityManager.AddComponentData(spawnedEntity, grid);
        state.EntityManager.DestroyEntity(gridSpawnerEntity);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (grid.gridNodes.IsCreated)
        {
            grid.gridNodes.Dispose();
        }
    }
}