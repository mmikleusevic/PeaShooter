using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity player)) return;

        PlayerMovementAspect playerMovement = SystemAPI.GetAspect<PlayerMovementAspect>(player);

        float3 moveDirection = new float3(playerMovement.input.ValueRO.move.x, 0, playerMovement.input.ValueRO.move.y);

        if (math.lengthsq(moveDirection) > 0f)
        {
            playerMovement.physics.ValueRW.Linear += moveDirection * playerMovement.playerController.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;

            quaternion targetRotation = quaternion.LookRotationSafe(new float3(moveDirection.x, 0, moveDirection.z), math.up());
            quaternion currentRotation = playerMovement.transform.ValueRO.Rotation;
            quaternion rotationDifference = math.mul(targetRotation, math.inverse(currentRotation));

            float3 eulerDifference = math.degrees(new float3(math.asin(rotationDifference.value.x) * 2, math.asin(rotationDifference.value.y) * 2, math.asin(rotationDifference.value.z) * 2));
            float3 angularVelocity = new float3(0, eulerDifference.y, 0) * playerMovement.playerController.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime;

            playerMovement.physics.ValueRW.Angular = angularVelocity;
            playerMovement.transform.ValueRW.Rotation = math.slerp(currentRotation, targetRotation, playerMovement.playerController.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
        }
        else
        {
            playerMovement.physics.ValueRW.Angular = 0;
        }

        playerMovement.playerController.ValueRW.moveDirection = moveDirection;

        float3 currentPosition = playerMovement.transform.ValueRO.Position;

        currentPosition.z = math.clamp(currentPosition.z, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());
        currentPosition.y = 0.5f;
        currentPosition.x = math.clamp(currentPosition.x, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());

        playerMovement.transform.ValueRW.Position = currentPosition;
    }
}
