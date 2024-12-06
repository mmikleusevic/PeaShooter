#region

using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

#endregion

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct PathfindingSystem : ISystem
    {
        private EntityQuery playerEntityQuery;
        private EntityQuery gridEntityQuery;
        private EntityQuery inputEntityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerComponent, PlayerAliveComponent>()
                .Build(ref state);

            gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GridComponent>()
                .Build(ref state);

            inputEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<InputComponent>()
                .Build(ref state);

            state.RequireForUpdate(playerEntityQuery);
            state.RequireForUpdate(gridEntityQuery);
            state.RequireForUpdate<EnemyComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int2 playerPosition = playerEntityQuery.GetSingleton<PlayerComponent>().gridPosition;
            GridComponent grid = gridEntityQuery.GetSingleton<GridComponent>();

            PathfindingJob job = new PathfindingJob
            {
                elapsedTime = (float)SystemAPI.Time.ElapsedTime,
                defaultMoveSpeed = 100f,
                playerPosition = playerPosition,
                gridNodes = grid.gridNodes,
                inputComponent = inputEntityQuery.GetSingleton<InputComponent>()
            };

            JobHandle handle = job.ScheduleParallel(state.Dependency);
            state.Dependency = handle;
        }
    }
}