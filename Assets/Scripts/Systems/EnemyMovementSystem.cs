using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct EnemyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity)) return;

        float deltaTime = SystemAPI.Time.DeltaTime;
        float3 playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

        EnemyMovementJob enemyMovementJob = new EnemyMovementJob
        {
            PlayerPosition = playerPosition,
            DeltaTime = deltaTime
        };

        JobHandle handle = enemyMovementJob.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}