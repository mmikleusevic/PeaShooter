using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(BeginInitializationEntityCommandBufferSystem))]
    [UpdateAfter(typeof(PlayerSpawnerSystem))]
    public partial struct ObstacleSpawnerSystem : ISystem
    {
        private EntityQuery gridEntityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GridComponent>()
                .Build(ref state);

            state.RequireForUpdate(gridEntityQuery);
            state.RequireForUpdate<ObstacleSpawnerComponent>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();

            uint seed = math.hash(new int2(Time.frameCount, (int)(SystemAPI.Time.ElapsedTime * 1000)));

            ObstacleSpawnJob job = new ObstacleSpawnJob
            {
                ecb = ecb,
                gridNodes = gridComponent.gridNodes,
                seed = seed
            };

            JobHandle spawnHandle = job.Schedule(state.Dependency);
            state.Dependency = spawnHandle;
        }
    }
}