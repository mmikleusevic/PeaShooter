using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup), OrderLast = true)]
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
                .WithAll<GridEnemyPositionUpdateComponent, PositionChangedComponent>()
                .Build(ref state);

            changedEntityQuery.SetChangedVersionFilter(ComponentType.ReadOnly<GridEnemyPositionUpdateComponent>());
            gridEnemyPositionUpdateComponentTypeHandle =
                state.GetComponentTypeHandle<GridEnemyPositionUpdateComponent>(true);
            gridEnemyPositionUpdateTypeHandle = state.GetEntityTypeHandle();
            removalPositions = new NativeParallelMultiHashMap<int2, Entity>(2000, Allocator.Persistent);

            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerAliveComponent>();
            state.RequireForUpdate(gridEntityQuery);
            state.RequireForUpdate(changedEntityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (changedEntityQuery.IsEmpty) return;

            EndFixedStepSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();
            gridEnemyPositionUpdateComponentTypeHandle.Update(ref state);
            gridEnemyPositionUpdateTypeHandle.Update(ref state);

            GridAddEnemyToPositionFlagItJob gridAddEnemyAndFlagJob = new GridAddEnemyToPositionFlagItJob
            {
                ecb = ecb.AsParallelWriter(),
                enemyPositionsWriter = gridComponent.enemyPositions.AsParallelWriter(),
                removalPositionsParallel = removalPositions.AsParallelWriter(),
                GridEnemyPositionUpdateTypeHandle = gridEnemyPositionUpdateComponentTypeHandle,
                EntityTypeHandle = gridEnemyPositionUpdateTypeHandle
            };

            JobHandle addAndFlagJobHandle =
                gridAddEnemyAndFlagJob.ScheduleParallel(changedEntityQuery, state.Dependency);

            GridEnemyRemoveFromPositionJob gridEnemyRemoveJob = new GridEnemyRemoveFromPositionJob
            {
                enemyPositions = gridComponent.enemyPositions,
                entitiesOnPositionsToRemove = removalPositions
            };

            JobHandle removeEntityJobHandle = gridEnemyRemoveJob.Schedule(addAndFlagJobHandle);

            state.Dependency = removeEntityJobHandle;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            removalPositions.Dispose();
        }
    }
}