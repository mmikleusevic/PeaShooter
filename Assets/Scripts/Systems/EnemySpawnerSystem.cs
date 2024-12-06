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
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial struct EnemySpawnerSystem : ISystem
    {
        private EntityQuery gridEntityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GridComponent>()
                .Build(ref state);

            state.RequireForUpdate(gridEntityQuery);
            state.RequireForUpdate<EnemySpawnerComponent>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerAliveComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();

            EndInitializationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            uint seed = math.hash(new int2(Time.frameCount, (int)(SystemAPI.Time.ElapsedTime * 1000)));

            EnemySpawnJob job = new EnemySpawnJob
            {
                ecb = ecb.AsParallelWriter(),
                elapsedTime = SystemAPI.Time.ElapsedTime,
                gridNodes = gridComponent.gridNodes,
                seed = seed
            };

            JobHandle handle = job.Schedule(state.Dependency);
            state.Dependency = handle;
        }
    }
}