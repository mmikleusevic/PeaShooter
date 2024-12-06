#region

using Components;
using Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

#endregion

namespace Jobs
{
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public int2 size;

        private void Execute(ref PlayerComponent playerComponent, in InputComponent inputComponent,
            ref LocalTransform localTransform,
            ref PhysicsVelocity physicsVelocity)
        {
            float3 moveDirection = new float3(inputComponent.moveInput.x, 0, inputComponent.moveInput.y);

            if (!MathExtensions.Approximately(moveDirection, 0))
            {
                quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
                localTransform.Rotation = math.slerp(
                    localTransform.Rotation,
                    targetRotation,
                    playerComponent.rotationSpeed * deltaTime
                );
            }

            physicsVelocity.Linear = moveDirection * playerComponent.moveSpeed * deltaTime;
            localTransform.Position = math.clamp(localTransform.Position, -size.x, size.x);
            playerComponent.gridPosition = new int2((int)math.round(localTransform.Position.x),
                (int)math.round(localTransform.Position.z));
            playerComponent.position = localTransform.Position;
        }
    }
}