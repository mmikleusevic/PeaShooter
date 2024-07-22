using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(EnemySpawnerSystem))]
public partial struct PathfindingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        LocalTransform playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        foreach (var LocalTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyComponent>())
        {
            PathfindingJob job = new PathfindingJob
            {
                position = LocalTransform.ValueRO.Position,
                playerPosition = playerTransform.Position
            };

            JobHandle handle = job.ScheduleParallel(state.Dependency);
            state.Dependency = handle;
        }
    }
}