using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial struct GridRemoveDeadEnemyPositionsSystem : ISystem
{
    private EntityQuery gridEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<GridComponent>()
            .Build(ref state);

        state.RequireForUpdate(gridEntityQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RefRW<GridComponent> gridComponent = gridEntityQuery.GetSingletonRW<GridComponent>();

        foreach ((RefRO<EnemyComponent> enemyPositionUpdate, Entity entity) in SystemAPI.Query<RefRO<EnemyComponent>>()
                     .WithAll<EnemyDeadComponent>()
                     .WithEntityAccess())
            RemoveEnemyFromPosition(gridComponent.ValueRW.enemyPositions, enemyPositionUpdate.ValueRO.gridPosition,
                entity);
    }

    [BurstCompile]
    public void RemoveEnemyFromPosition(NativeHashMap<int2, NativeList<Entity>> enemyPositions, int2 position,
        Entity other)
    {
        if (enemyPositions.TryGetValue(position, out NativeList<Entity> list))
            for (int i = 0; i < list.Length; i++)
            {
                Entity enemy = list[i];

                if (enemy.Equals(other))
                {
                    list.RemoveAt(i);
                    enemyPositions[position] = list;
                    break;
                }
            }
    }
}