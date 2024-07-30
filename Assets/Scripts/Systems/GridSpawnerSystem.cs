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
            gridNodes = new NativeHashMap<int2, bool>(spawner.ValueRO.size.x * spawner.ValueRO.size.y, Allocator.Persistent)
        };

        for (int i = -spawner.ValueRO.size.x + 1; i < spawner.ValueRO.size.x; i++)
        {
            for (int j = -spawner.ValueRO.size.y + 1; j < spawner.ValueRO.size.y; j++)
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
        NativeArray<Entity> entities = state.GetEntityQuery(ComponentType.ReadOnly<GridComponent>()).ToEntityArray(Allocator.Temp);

        foreach (var entity in entities)
        {
            var gridComponent = state.EntityManager.GetComponentData<GridComponent>(entity);
            gridComponent.gridNodes.Dispose();
        }

        entities.Dispose();
    }
}