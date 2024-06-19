using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MovePlayer(ref state);
    }

    [BurstCompile]
    private void MovePlayer(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity player)) return;

        PlayerMovementAspect playerMovement = SystemAPI.GetAspect<PlayerMovementAspect>(player);

        float3 moveDirection = new float3(playerMovement.input.ValueRO.move.x, playerMovement.input.ValueRO.move.y, 0);

        playerMovement.physics.ValueRW.Linear += moveDirection * playerMovement.playerController.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;

        playerMovement.playerController.ValueRW.moveDirection = moveDirection;

        float3 currentPosition = playerMovement.transform.ValueRO.Position;

        currentPosition.x = math.clamp(currentPosition.x, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());
        currentPosition.y = math.clamp(currentPosition.y, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());

        playerMovement.transform.ValueRW.Position = currentPosition;
    }
}
