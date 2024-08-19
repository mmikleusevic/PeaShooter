using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(CollisionDamageSystem))]
public partial struct PlayerMovementSystem : ISystem
{
    private GridSpawnerComponent gridSpawner;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<GridSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerMovementJob job = new PlayerMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            gridSpawner = SystemAPI.GetSingleton<GridSpawnerComponent>()
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}