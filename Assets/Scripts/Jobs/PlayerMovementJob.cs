using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerMovementJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public GridComponent gridComponent;

    private void Execute(ref PlayerComponent player, in InputComponent input, ref LocalTransform transform, ref PhysicsVelocity velocity)
    {
        float3 moveDirection = new float3(input.move.x, 0, input.move.y);

        if (!MathExtensions.Approximately(moveDirection, 0))
        {
            quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
            transform.Rotation = math.slerp(
                transform.Rotation,
                targetRotation,
                player.rotationSpeed * deltaTime
            );
        }

        velocity.Linear = moveDirection * player.moveSpeed * deltaTime;
        transform.Position = math.clamp(transform.Position, -gridComponent.size.x, gridComponent.size.x);
        player.position = new int2((int)math.round(transform.Position.x), (int)math.round(transform.Position.z));
    }
}