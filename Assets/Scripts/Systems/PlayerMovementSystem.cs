using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerMovementJob playerMovementJob = new PlayerMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        playerMovementJob.Schedule();
    }
}
