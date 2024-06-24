using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity player)) return;

        PlayerMovementAspect playerMovement = SystemAPI.GetAspect<PlayerMovementAspect>(player);

        float3 moveDirection = new float3(playerMovement.input.ValueRO.move.x, 0, playerMovement.input.ValueRO.move.y);

        if (!MathExtensions.Approximately(moveDirection, 0))
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z), Vector3.up);

            playerMovement.transform.ValueRW.Rotation = Quaternion.Slerp(playerMovement.transform.ValueRO.Rotation, targetRotation,
                playerMovement.playerController.ValueRO.rotationSpeed * Time.deltaTime);
        }

        playerMovement.physics.ValueRW.Linear = moveDirection * playerMovement.playerController.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;
    }
}
