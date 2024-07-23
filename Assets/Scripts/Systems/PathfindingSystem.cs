using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PathfindingSystem : ISystem
{
    private EntityQuery obstacleQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();
        state.RequireForUpdate<ObstacleComponent>();

        obstacleQuery = new EntityQueryBuilder(Allocator.Persistent)
            .WithAll<ObstacleComponent>()
            .Build
            (ref state);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        NativeArray<ObstacleComponent> obstacles = obstacleQuery.ToComponentDataArray<ObstacleComponent>(Allocator.Persistent);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        LocalTransform playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        foreach (var localTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyComponent>())
        {
            PathfindingJob job = new PathfindingJob
            {
                position = new float2
                {
                    x = localTransform.ValueRO.Position.x,
                    y = localTransform.ValueRO.Position.z
                },
                playerPosition = new float2
                {
                    x = playerTransform.Position.x,
                    y = playerTransform.Position.z
                },
            };

            JobHandle handle = job.ScheduleParallel(state.Dependency);
            state.Dependency = handle;
        }

        obstacles.Dispose();
    }
}