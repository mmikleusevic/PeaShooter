using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
    private EntityQuery gridEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(ref state);

        state.RequireForUpdate(gridEntityQuery);
        state.RequireForUpdate<PlayerAliveComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerMovementJob job = new PlayerMovementJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            size = gridEntityQuery.GetSingleton<GridComponent>().size,
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}