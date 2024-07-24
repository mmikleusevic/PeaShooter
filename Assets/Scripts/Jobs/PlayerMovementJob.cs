using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerMovementJob : IJobEntity
{
    public float DeltaTime;

    void Execute(PlayerMovementAspect playerMovement)
    {
        float3 moveDirection = new float3(playerMovement.input.ValueRO.move.x, 0, playerMovement.input.ValueRO.move.y);

        if (!MathExtensions.Approximately(moveDirection, 0))
        {
            quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
            playerMovement.transform.ValueRW.Rotation = math.slerp(
                playerMovement.transform.ValueRO.Rotation,
                targetRotation,
                playerMovement.player.ValueRO.rotationSpeed * DeltaTime
            );
        }

        playerMovement.player.ValueRW.position = new float2(playerMovement.transform.ValueRO.Position.x, playerMovement.transform.ValueRO.Position.y);
        playerMovement.physics.ValueRW.Linear = moveDirection * playerMovement.player.ValueRO.moveSpeed * DeltaTime;
    }
}