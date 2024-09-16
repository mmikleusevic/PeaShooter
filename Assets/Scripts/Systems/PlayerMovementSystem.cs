using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
[WithAll(typeof(PlayerComponent))]
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
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerMovementJob job = new PlayerMovementJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            gridComponent = gridEntityQuery.GetSingleton<GridComponent>(),
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}