using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderFirst = true)]
public partial struct GridEnemyPositionUpdateSystem : ISystem
{
    private EntityQuery gridEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<GridComponent>()
            .Build(ref state);

        state.RequireForUpdate<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(gridEntityQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BeginVariableRateSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        RefRW<GridComponent> gridComponent = gridEntityQuery.GetSingletonRW<GridComponent>();

        foreach (var (gridEnemyPositionUpdate, entity) in SystemAPI.Query<RefRW<GridEnemyPositionUpdateComponent>>()
            .WithEntityAccess())
        {
            if (gridEnemyPositionUpdate.ValueRO.status == UpdateStatus.Move)
            {
                MoveEnemyToPosition(gridComponent.ValueRW.enemyPositions, gridEnemyPositionUpdate.ValueRO.oldPosition, gridEnemyPositionUpdate.ValueRO.position, gridEnemyPositionUpdate.ValueRO.entity);
            }
            else
            {
                AddEnemyToPosition(gridComponent.ValueRW.enemyPositions, gridEnemyPositionUpdate.ValueRO.position, gridEnemyPositionUpdate.ValueRO.entity);
            }

            ecb.RemoveComponent<GridEnemyPositionUpdateComponent>(entity);
        }
    }

    [BurstCompile]
    public void AddEnemyToPosition(NativeHashMap<int2, NativeList<Entity>> enemyPositions, int2 position, Entity enemy)
    {
        if (!enemyPositions.TryGetValue(position, out NativeList<Entity> list))
        {
            list = new NativeList<Entity>(Allocator.Persistent);
        }

        list.Add(enemy);

        if (enemyPositions.ContainsKey(position))
        {
            enemyPositions[position] = list;
        }
        else
        {
            enemyPositions.Add(position, list);
        }
    }

    [BurstCompile]
    public void MoveEnemyToPosition(NativeHashMap<int2, NativeList<Entity>> enemyPositions, int2 oldPosition, int2 position, Entity other)
    {
        if (enemyPositions.TryGetValue(position, out NativeList<Entity> list))
        {
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

        AddEnemyToPosition(enemyPositions, position, other);
    }
}