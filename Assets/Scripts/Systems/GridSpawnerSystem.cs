using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial struct GridSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        Entity entity = SystemAPI.GetSingletonEntity<GridSpawnerComponent>();

        RefRO<GridSpawnerComponent> spawner = SystemAPI.GetComponentRO<GridSpawnerComponent>(entity);

        Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.prefab);

        GridComponent grid = new GridComponent
        {
            gridNodes = new NativeHashMap<int2, bool>(math.square(spawner.ValueRO.size.x + spawner.ValueRO.size.y + 1), Allocator.Persistent)
        };

        for (int i = -spawner.ValueRO.size.x; i <= spawner.ValueRO.size.x; i++)
        {
            for (int j = -spawner.ValueRO.size.y; j <= spawner.ValueRO.size.y; j++)
            {
                int2 position = new int2(i, j);
                grid.gridNodes[position] = true;
            }
        }

        state.EntityManager.AddComponentData(spawnedEntity, grid);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        foreach (var grid in SystemAPI.Query<RefRW<GridComponent>>())
        {
            grid.ValueRW.gridNodes.Dispose();
        }
    }
}