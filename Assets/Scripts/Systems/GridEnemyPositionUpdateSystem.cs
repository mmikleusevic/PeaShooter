using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[RequireMatchingQueriesForUpdate]
public partial struct GridEnemyPositionUpdateSystem : ISystem
{
    private EntityQuery gridEntityQuery;
    private EntityQuery changedEntityQuery;
    private ComponentTypeHandle<GridEnemyPositionUpdateComponent> gridEnemyPositionUpdateComponentTypeHandle;
    private EntityTypeHandle gridEnemyPositionUpdateTypeHandle;
    private NativeParallelMultiHashMap<int2, Entity> removalPositions;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(ref state);

        changedEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridEnemyPositionUpdateComponent, HasChangedPositionComponent>()
            .Build(ref state);

        changedEntityQuery.SetChangedVersionFilter(ComponentType.ReadOnly<GridEnemyPositionUpdateComponent>());
        gridEnemyPositionUpdateComponentTypeHandle =
            state.GetComponentTypeHandle<GridEnemyPositionUpdateComponent>(true);
        gridEnemyPositionUpdateTypeHandle = state.GetEntityTypeHandle();
        removalPositions = new NativeParallelMultiHashMap<int2, Entity>(2000, Allocator.Persistent);

        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PlayerAliveComponent>();
        state.RequireForUpdate(gridEntityQuery);
        state.RequireForUpdate(changedEntityQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (changedEntityQuery.IsEmpty) return;

        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();
        gridEnemyPositionUpdateComponentTypeHandle.Update(ref state);
        gridEnemyPositionUpdateTypeHandle.Update(ref state);

        AddEnemyToPositionFlagItJob addAndFlagJob = new AddEnemyToPositionFlagItJob
        {
            ecb = ecb.AsParallelWriter(),
            enemyPositionsWriter = gridComponent.enemyPositions.AsParallelWriter(),
            removalPositionsParallel = removalPositions.AsParallelWriter(),
            GridEnemyPositionUpdateTypeHandle = gridEnemyPositionUpdateComponentTypeHandle,
            EntityTypeHandle = gridEnemyPositionUpdateTypeHandle
        };

        JobHandle addAndFlagJobHandle = addAndFlagJob.ScheduleParallel(changedEntityQuery, state.Dependency);

        RemoveEntityFromPositionJob removeEntityJob = new RemoveEntityFromPositionJob
        {
            enemyPositions = gridComponent.enemyPositions,
            entitiesOnPositionsToRemove = removalPositions
        };

        JobHandle removeEntityJobHandle = removeEntityJob.Schedule(addAndFlagJobHandle);

        state.Dependency = removeEntityJobHandle;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        removalPositions.Dispose();
    }
}