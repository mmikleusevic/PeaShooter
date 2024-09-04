using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerMovementJob : IJobEntity
{
    public float deltaTime;
    public GridComponent gridComponent;

    private void Execute(PlayerMovementAspect playerMovement)
    {
        float3 moveDirection = new float3(playerMovement.input.ValueRO.move.x, 0, playerMovement.input.ValueRO.move.y);

        if (!MathExtensions.Approximately(moveDirection, 0))
        {
            quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
            playerMovement.transform.ValueRW.Rotation = math.slerp(
                playerMovement.transform.ValueRO.Rotation,
                targetRotation,
                playerMovement.player.ValueRO.rotationSpeed * deltaTime
            );
        }

        playerMovement.physics.ValueRW.Linear = moveDirection * playerMovement.player.ValueRO.moveSpeed * deltaTime;
        playerMovement.transform.ValueRW.Position = math.clamp(playerMovement.transform.ValueRO.Position, -gridComponent.size.x, gridComponent.size.x);
        playerMovement.player.ValueRW.position = new int2((int)math.round(playerMovement.transform.ValueRO.Position.x), (int)math.round(playerMovement.transform.ValueRO.Position.z));
    }
}